using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServ.Core.Interfaces
{
    /// <summary>
    /// Interface for chat rooms which handles the actual chat along with receiving + sedning messages.
    /// </summary>
    public interface IRoom
    {
        public string Id { get; }

        public string Name { get; }

        public bool ShouldRemove { get; }

        void AddConnection(WebSocket connection);

        Task AddMessage(string message);

        Task Process();

        Task CloseRoom(string message);
    }
}
