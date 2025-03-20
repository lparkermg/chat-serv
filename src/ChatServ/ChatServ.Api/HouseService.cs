
using ChatServ.Core;
using ChatServ.Core.Interfaces;
using ChatServ.Core.Models;

namespace ChatServ.Api
{
    public class HouseService(IHouse house) : IHostedService
    {
        private readonly IHouse _house = house;

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
