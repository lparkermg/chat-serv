using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServ.Core.Interfaces
{
    /// <summary>
    /// Interface for the chat house, handling various chat rooms and connections.
    /// </summary>
    public interface IHouse<T> where T : class
    {
        string AddRoom(string id, string name, bool removeOnEmpty);

        bool TryJoinRoom(string roomId, WebSocket connection);

        bool TryRemoveRoom(string id);

        Task CloseHouse();
    }
}
