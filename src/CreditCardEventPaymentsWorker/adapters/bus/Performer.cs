using System.Threading;
using System.Threading.Tasks;
using JustSaying;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JustRooms.DirectBookingEventConsumer.adapters.bus
{
    public class Performer : BackgroundService
    {
        private readonly IMessagingBus _bus;
        private readonly ILogger<Performer> _logger;

        public Performer(IMessagingBus bus, ILogger<Performer> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Direct Booking consumer running");

            _bus.Start(stoppingToken);

            return Task.CompletedTask;
        }
    }
}