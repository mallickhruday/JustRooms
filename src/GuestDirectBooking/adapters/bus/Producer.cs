using System.Threading;
using System.Threading.Tasks;
using JustRooms.GuestDirectBooking.ports.events;
using JustSaying.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JustRooms.GuestDirectBooking.adapters.bus
{
    public class Producer : BackgroundService
    {
        private const int PUMP_LIMIT = 100;
        private readonly IMessagePublisher _publisher;
        private readonly ILogger<Producer > _logger;

        public Producer (IMessagePublisher publisher, ILogger<Producer> logger)
        {
            _publisher = publisher;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Direct Booking producer running");

                for (int n = 0; n < PUMP_LIMIT; n++)
                {
                    await _publisher.PublishAsync(new GuestRoomBookingMade
                    {
                        Type = RoomType.King,
                        Price = new Money(150, "USD"),
                        NumberOfNights = 3,
                        NumberOfGuests = 2,
                        FirstName = "Jeffrey",
                        LastName = "Lebowski",
                        FistLineOfAddress = "5227 Santa Monica Boulevard",
                        ZipCode = "90029",
                        State = "CA",
                        TelephoneNumber = "537-3375",
                        CardNumber = "4012888888881881",
                        CardSecurityCode = "303"
                    });
                }
        }
    }
}