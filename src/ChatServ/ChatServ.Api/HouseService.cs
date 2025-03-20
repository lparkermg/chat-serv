
using ChatServ.Core;
using ChatServ.Core.Interfaces;
using ChatServ.Core.Models;

namespace ChatServ.Api
{
    public class HouseService(IHouse<BasicMessageDTO> house) : IHostedService
    {
        private readonly IHouse<BasicMessageDTO> _house = house;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _house.AddRoom("global", "Global Chat", false);

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _house.CloseHouse();
        }
    }
}
