using ChatServ.Core.Interfaces;
using ChatServ.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Net.WebSockets;
using System.Text;

namespace ChatServ.Api.Controllers
{
    public class ChatController(IHouse house, ILogger<ChatController> logging) : ControllerBase
    {
        private readonly IHouse _house = house;

        [Route("/chat/{roomId}")]
        public async Task Get([FromRoute] string roomId)
        {
            logging.LogInformation($"Request to join room {roomId}.");
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var socket = await HttpContext.WebSockets.AcceptWebSocketAsync("string");
                var success = _house.TryJoinRoom(roomId, socket);
                if (!success)
                {
                    logging.LogWarning($"Failed to join room.");
                    await HttpContext.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes("Room not found."));
                    HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                }
                else
                {
                    logging.LogInformation($"Successfully joined room, sending connected message.");
                    await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("CONNECTED")), WebSocketMessageType.Text, WebSocketMessageFlags.EndOfMessage, CancellationToken.None);
                    await ProcessIncomingMessagesAsync(socket, roomId);
                }
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private async Task ProcessIncomingMessagesAsync(WebSocket socket, string roomId)
        {
            var room = _house.FindRoom(roomId);

            if (room == null)
            {
                logging.LogError("Room id could not be found, closing connection.");
                await socket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Room not found", CancellationToken.None);
            }
            var buf = new byte[1024 * 4];
            var msgRes = await socket.ReceiveAsync(new ArraySegment<byte>(buf), CancellationToken.None);

            while (!socket.CloseStatus.HasValue)
            {
                await _house.SendMessageToRoom(roomId, Encoding.UTF8.GetString(buf).Replace("\u0000",""));
                msgRes = await socket.ReceiveAsync(new ArraySegment<byte>(buf), CancellationToken.None);
            }
            await socket.CloseAsync(msgRes.CloseStatus.Value, msgRes.CloseStatusDescription, CancellationToken.None);
        }

        
    }
}
