using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime.CredentialManagement;
using JustRooms.GuestDirectBooking.ports.events;
using JustSaying;
using JustSaying.AwsTools;
using Microsoft.Extensions.Logging.Abstractions;

namespace JustRooms.GuestDirectBooking
{
    class Program
    {
        private const int PUMP_LIMIT = 100;
        
        static async Task Main(string[] args)
        {
            if (new CredentialProfileStoreChain().TryGetAWSCredentials("default", out var credentials))
            {
                CreateMeABus.DefaultClientFactory = () => new DefaultAwsClientFactory(credentials);
                var publisher = CreateMeABus.WithLogging(NullLoggerFactory.Instance)
                    .InRegion(RegionEndpoint.EUWest1.SystemName)
                    .WithSnsMessagePublisher<GuestRoomBookingMade>();

                for (int n = 0; n < PUMP_LIMIT; n++)
                {
                    await publisher.PublishAsync(new GuestRoomBookingMade
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
}
