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
    public class BasicNonTextRoom(ILogger<IRoom> logger, string id, string name, bool shouldRemoveOnNoConnections, string[] validMessages) : IRoom
    {
        public string Id { get; private set; } = id;
        public string Name { get; private set; } = name;

        public bool ShouldRemove { get; private set; } = false;

        private readonly Channel<BasicNonTextMessageDTO> _channel = Channel.CreateUnbounded<BasicNonTextMessageDTO>();
        private readonly ILogger<IRoom> _logger = logger;

        private readonly IList<WebSocket> _connections = new List<WebSocket>();

        private readonly bool _removeOnNoConnections = shouldRemoveOnNoConnections;

        private readonly string[] _validMessages = validMessages;

        public void AddConnection(WebSocket connection)
        {
            _logger.LogDebug("Adding connection to room {id}.", Id);

            _connections.Add(connection);

            _logger.LogDebug("Connection added to room {id}.", Id);
        }

        public async Task Process()
        {
            _logger.LogDebug("Processing room {id}", Id);

            // Remove any closed connections
            var closedConnections = _connections.Where(c => c.CloseStatus.HasValue);

            _logger.LogDebug("Removing closed connections.");
            foreach (var connection in closedConnections)
            {
                _connections.Remove(connection);
            }
            _logger.LogDebug("Removed all closed conections.");

            if (_removeOnNoConnections && _connections.Count() == 0)
            {
                _logger.LogDebug("Flagging room for removal due to no connections.");
                ShouldRemove = true;
                return;
            }

            // Receive any messages from connections.
            _logger.LogDebug("Receiving messages from connections.");
            var buf = new byte[1024 * 4];
            WebSocketReceiveResult? result = null;
            foreach (var connection in _connections)
            {
                result = await connection.ReceiveAsync(new ArraySegment<byte>(buf), CancellationToken.None);

                var data = JsonSerializer.Deserialize<BasicNonTextMessageDTO>(buf) ?? null;

                if (data != null && _validMessages.Length <= data.Content)
                {
                    await _channel.Writer.WriteAsync(data);
                }
            }
            _logger.LogDebug("Received messages from connections.");

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
            foreach(var connection in _connections)
            {
                await connection.CloseAsync(WebSocketCloseStatus.NormalClosure, $"Closing Room: {message}", CancellationToken.None);
            }
        }
    }
}
