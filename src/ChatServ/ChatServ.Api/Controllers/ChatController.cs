using ChatServ.Core.Interfaces;
using ChatServ.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace ChatServ.Api.Controllers
{
    public class ChatController(IHouse house) : ControllerBase
    {
        private readonly IHouse _house = house;

        [Route("/chat/{roomId}")]
        public async Task Get([FromRoute] string roomId)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                var success = _house.TryJoinRoom(roomId, socket);

                if (!success)
                {
                    await HttpContext.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes("Room not found."));
                    HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                }
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
