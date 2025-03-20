using ChatServ.Api.Configuration;
using ChatServ.Api.Models.Requests;
using ChatServ.Core.Interfaces;
using ChatServ.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ChatServ.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HouseController(IHouse house, IOptions<ApiOptions> options) : ControllerBase
    {
        private readonly IHouse _house = house;
        private readonly ApiOptions _apiOptions = options.Value;

        [HttpGet("room/{roomId}")]
        public IActionResult GetRoomUrl(string roomId)
        {
            if (!_house.DoesRoomExist(roomId))
            {
                return NotFound();
            }
            return Ok($"{_apiOptions.ChatBaseUrl}chat/{roomId}");
        }

        [HttpPost("room")]
        public IActionResult CreateRoom([FromBody] CreateRoomRequest room)
        {
            _house.AddRoom(room.Id, room.Name, true);
            return Created();
        }
    }
}
