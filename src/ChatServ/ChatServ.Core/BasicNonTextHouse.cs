using ChatServ.Core.Configuration;
using ChatServ.Core.Interfaces;
using ChatServ.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChatServ.Core
{
    public class BasicNonTextHouse : IHouse
    {
        private IList<IRoom> _rooms = new List<IRoom>();

        private readonly ILogger<BasicNonTextHouse> _logger;
        private readonly ILogger<IRoom> _roomLogger;

        private readonly BasicNonTextHouseOptions _options;

        public BasicNonTextHouse(ILogger<BasicNonTextHouse> logger, ILogger<IRoom> roomLogger, IOptions<BasicNonTextHouseOptions> options)
        {
            _logger = logger;
            _roomLogger = roomLogger;

            _options = options.Value;
        }

        public string AddRoom(string id, string name, bool removeOnEmpty)
        {
            if(_rooms.Any(r => r.Id == id))
            {
                _logger.LogWarning("Room with id {id} already exists.", id);
                return id;
            }

            var room = new BasicNonTextRoom(_roomLogger, id, name, removeOnEmpty, _options.AvailableMessages);
            _rooms.Add(room);

            return id;
        }

        private IRoom? FindRoom(string id)
        {
            var room = _rooms.SingleOrDefault(r => r.Id == id);

            if (room == null)
            {
                _logger.LogWarning("Room with id {id} not found.", id);
                return null;
            }

            return room;
        }

        public bool TryRemoveRoom(string id)
        {
            var room = FindRoom(id);

            if (room == null)
            {
                return false;
            }

            _rooms.Remove(room);

            return true;
        }

        public async Task ProcessHouse()
        {
            foreach (var room in _rooms)
            {
                await room.Process();
            }

            var emptyRooms = _rooms.Where(r => r.ShouldRemove);

            foreach (var room in emptyRooms)
            {
                _rooms.Remove(room);
            }
        }

        public bool TryJoinRoom(string roomId, WebSocket connection)
        {
            var room = FindRoom(roomId);

            if (room == null)
            {
                _logger.LogWarning("Room with id {id} not found.", roomId);
                return false;
            }

            room.AddConnection(connection);
            return true;
        }

        public async Task CloseHouse()
        {
            _logger.LogDebug($"CloseHouse has been called, closing all rooms.");
            foreach(var room in _rooms)
            {
                await room.CloseRoom("House is closing down.");
            }
            _logger.LogDebug($"All rooms have been closed.");
        }

        public bool DoesRoomExist(string roomId)
        {
            return _rooms.Any(r => r.Id == roomId);
        }
    }
}
