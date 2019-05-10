using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using JustRooms.DirectBookingEventConsumer.adapters.di;
using JustRooms.DirectBookingEventConsumer.ports.events;
using JustSaying;
using JustSaying.AwsTools;
using Microsoft.Extensions.Logging.Abstractions;

namespace JustRooms.DirectBookingEventConsumer.adapters.bus
{
    public class StartUp
    {
        private AWSCredentials _awsCredentials;
        
        public StartUp()
        {
            new CredentialProfileStoreChain().TryGetAWSCredentials("default", out _awsCredentials);
        }

        public void Listen()
        {
            CreateMeABus.DefaultClientFactory = () => new DefaultAwsClientFactory(_awsCredentials);
            CreateMeABus.WithLogging(NullLoggerFactory.Instance)
                .InRegion(RegionEndpoint.EUWest1.SystemName)
                .WithNamingStrategy(() => new TypeNameIndepedentNamingStrategy())
                .WithSqsTopicSubscriber()
                .IntoQueue(Globals.BOOKING_QUEUE_NAME)
                .ConfigureSubscriptionWith(c =>
                {
                    c.MaxAllowedMessagesInFlight = 10;
                    c.OnError = (exception, message) =>
                    {
                        /*don't bubble out message*/
                    };
                })
                .WithMessageHandler<RoomBookingMade>(new HandlerResolver())
                .StartListening();
        }
     }
}