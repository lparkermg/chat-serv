using ChatServ.Core.Interfaces;
using ChatServ.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatServ.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HouseController(IHouse house) : ControllerBase
    {
        private readonly IHouse _house = house;
        // TODO: Options for getting base url etc

        [HttpGet("room/{roomId}")]
        public IActionResult GetRoomUrl(string roomId)
        {
            if (!_house.DoesRoomExist(roomId))
            {
                return NotFound();
            }
            return Ok($"/chat/{roomId}");
        }
    }
}
