using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatServ.Api.Controllers
{
    public class ChatController : ControllerBase
    {
        [Route("/chat")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                // TODO: Add socket to connection handler, this should return a connection id.
                // Connection Handler should add and remove connections + receive and send via connections.
                // Connection handler should receive data, where a delegate would link it to the correct channel.
                // Connection handler should allow data to be sent.
                
                // TODO: Pass connection id to Receiver + Sender
                // Receiver constantly reads the socket and adds to the queue of messages (takes in ChannelWriter)
                // Sender formats and calls a delegeate to send the message to the correct set of connections.
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
