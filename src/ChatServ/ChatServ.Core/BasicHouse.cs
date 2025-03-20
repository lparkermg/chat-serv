using ChatServ.Core.Interfaces;
using ChatServ.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChatServ.Core
{
    public class BasicHouse<T> : IHouse<T> where T : class
    {
        private IList<IRoom<T>> _rooms = new List<IRoom<T>>();

        private readonly ILogger<BasicHouse<T>> _logger;
        private readonly ILogger<BasicRoom<T>> _roomLogger;

        public BasicHouse(ILogger<BasicHouse<T>> logger, ILogger<BasicRoom<T>> roomLogger)
        {
            _logger = logger;
            _roomLogger = roomLogger;
        }

        public string AddRoom(string id, string name, bool removeOnEmpty)
        {
            var room = new BasicRoom<T>(_roomLogger, id, name, removeOnEmpty);
            _rooms.Add(room);

            return id;
        }

        public IRoom<T>? FindRoom(string id)
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
            var room = _rooms.SingleOrDefault(r => r.Id == id);

            if (room == null)
            {
                _logger.LogWarning("Room with id {id} not found.", id);
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
    }
}
