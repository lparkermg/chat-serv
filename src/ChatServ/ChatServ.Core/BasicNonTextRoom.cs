using ChatServ.Core.Interfaces;
using ChatServ.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ChatServ.Core
{
    /// <summary>
    /// Basic implementation of a chat room.
    /// </summary>
    /// <typeparam name="T">The type of message the room will handle.</typeparam>
    /// <param name="logger">Logging for the room.</param>
    public class BasicNonTextRoom(ILogger<BasicNonTextRoom> logger, string id, string name, bool shouldRemoveOnNoConnections, string[] validMessages) : IRoom
    {
        public string Id { get; private set; } = id;
        public string Name { get; private set; } = name;

        public bool ShouldRemove { get; private set; } = false;

        // This should be set via options, but hardcoding for now.
        private DateTime? _shouldRemoveAt = DateTime.UtcNow.AddSeconds(30);

        private readonly Channel<BasicNonTextMessageDTO> _channel = Channel.CreateUnbounded<BasicNonTextMessageDTO>();
        private readonly ILogger<BasicNonTextRoom> _logger = logger;

        private readonly IList<WebSocket> _connections = new List<WebSocket>();

        private readonly bool _removeOnNoConnections = shouldRemoveOnNoConnections;

        private readonly string[] _validMessages = validMessages;

        public void AddConnection(WebSocket connection)
        {
            _logger.LogDebug("Adding connection to room {id}.", Id);

            _connections.Add(connection);
            _shouldRemoveAt = null;

            _logger.LogDebug("Connection added to room {id}.", Id);
        }

        public async Task AddMessage(string message)
        {
            try
            {
                _logger.LogInformation(message);
                var parsedMessage = JsonSerializer.Deserialize<BasicNonTextMessageDTO>(message, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });

                if (parsedMessage == null)
                {
                    _logger.LogWarning("Invalid message ({message}) could not be added to room {id}", message, Id);
                    return;
                }

                await _channel.Writer.WriteAsync(parsedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding message.");
            }
        }

        public async Task Process()
        {
            _logger.LogDebug("Processing room {id}", Id);

            // No one has connected within the time limit, so we should remove the room.
            if (_removeOnNoConnections && _shouldRemoveAt.HasValue && _shouldRemoveAt.Value < DateTime.UtcNow)
            {
                _logger.LogDebug("Flagging room for removal due to timeout.");
                ShouldRemove = true;
                return;
            }

            // Remove any closed connections
            var closedConnections = _connections.Where(c => c.CloseStatus.HasValue || c.State == WebSocketState.Closed).ToList();

            _logger.LogDebug("Removing closed connections. {amount}", closedConnections.Count);
            foreach (var connection in closedConnections)
            {
                _connections.Remove(connection);
            }
            _logger.LogDebug("Removed all closed conections.");

            if (_removeOnNoConnections && _connections.Count() == 0 && _shouldRemoveAt == null)
            {
                _logger.LogDebug("Flagging room for removal due to no connections.");
                ShouldRemove = true;
                return;
            }

            // Send any message to connections.
            _logger.LogDebug("Sending messages to connections.");
            var msg = string.Empty;
            while (_channel.Reader.TryRead(out var msgData))
            {
                msg = $"{msgData.Sender}: {_validMessages[msgData.Content]}";
                foreach(var connection in _connections)
                {
                    await connection.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg)), WebSocketMessageType.Text, WebSocketMessageFlags.EndOfMessage, CancellationToken.None);
                }
            }
            _logger.LogDebug("Sent messages to connections.");
            _logger.LogDebug("Room {id} processed.", Id);
        }

        public async Task CloseRoom(string message)
        {
            _logger.LogDebug("Closing room {id}.", Id);
            foreach (var connection in _connections)
            {
                await connection.CloseAsync(WebSocketCloseStatus.NormalClosure, $"Closing Room: {message}", CancellationToken.None);
            }
            _logger.LogDebug("Room {id} closed.", Id);
        }
    }
}
