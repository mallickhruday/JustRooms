using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime.CredentialManagement;
using JustRooms.DirectBookingEventConsumer.adapters.bus;
using JustRooms.DirectBookingEventConsumer.Ports.events;
using JustRooms.DirectBookingEventConsumer.Ports.handlers;
using JustSaying.AwsTools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JustRooms.DirectBookingEventConsumer
{
    class Program
    {
        public static async Task Main()
        {
            Console.Title = "Direct Booking Event Consumer";
            new CredentialProfileStoreChain().TryGetAWSCredentials("default", out var awsCredentials);
            
            await new HostBuilder()
                .ConfigureLogging(loggingBuilder => loggingBuilder.AddConsole())
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddJustSaying((config) =>
                    {
                        config.Client(x => { x.WithClientFactory(() => new DefaultAwsClientFactory(awsCredentials)); });
                        config.Messaging(x =>
                        {
                            x.WithRegions(RegionEndpoint.EUWest1.SystemName);
                        });
                        config.Subscriptions(x => { x.ForTopic<GuestRoomBookingMade>("guestroombookingmade"); });
                    });
                    services.AddJustSayingHandler<GuestRoomBookingMade, RoomBookingMadeHandler>();
                    services.AddHostedService<Performer>();
                })
                .UseConsoleLifetime()
                .Build()
                .RunAsync();
        }

   }
}