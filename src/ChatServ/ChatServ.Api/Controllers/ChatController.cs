using ChatServ.Core.Interfaces;
using ChatServ.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ChatServ.Api.Controllers
{
    public class ChatController(IHouse<BasicMessageDTO> house) : ControllerBase
    {
        private readonly IHouse<BasicMessageDTO> _house = house;

        [Route("/chat/{roomId}")]
        public async Task Get([FromRoute] string roomId)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                _house.TryJoinRoom(roomId, socket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
