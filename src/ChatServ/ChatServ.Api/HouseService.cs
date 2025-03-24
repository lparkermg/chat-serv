
using ChatServ.Core;
using ChatServ.Core.Interfaces;
using ChatServ.Core.Models;

namespace ChatServ.Api
{
    public class HouseService(IHouse house, ILogger<HouseService> logger) : BackgroundService
    {
        private readonly IHouse _house = house;
        private readonly ILogger<HouseService> _logger = logger;

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting house service.");
            _house.AddRoom("global", "Global Chat", false);
            _logger.LogDebug("Added persistant room (global).");

            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Stopping house service.");
            await _house.CloseHouse();
            _logger.LogDebug("House service stopped.");
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Starting house processing.");
            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await _house.ProcessHouse();
                }
            }
            catch (OperationCanceledException)
            {
                // Ignore
                _logger.LogInformation("Stopping house processing.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the house.");
            }
        }
    }
}
