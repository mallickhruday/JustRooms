using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime.CredentialManagement;
using JustRooms.DirectBookingEventConsumer.adapters.bus;
using JustRooms.DirectBookingEventConsumer.ports.events;
using JustRooms.DirectBookingEventConsumer.ports.handlers;
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
            
            var host = new HostBuilder()
                .ConfigureLogging(loggingBuilder => loggingBuilder.AddConsole())
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;
                    services.AddJustSaying((config) =>
                    {
                        config.Client(x => { x.WithClientFactory(() => new DefaultAwsClientFactory(awsCredentials)); });
                        config.Messaging(x =>
                        {
                            x.WithRegions(RegionEndpoint.EUWest1.SystemName);
                        });
                        config.Subscriptions(x => { x.ForQueue<RoomBookingMade>("").ForTopic<RoomBookingMade>("guestroombookingmade"); });
                        //config.Publications(x => { x.WithTopic(); }); --can't override to string, which prevents client creating topic with different name
                    });
                    services.AddJustSayingHandler<RoomBookingMade, RoomBookingMadeHandler>();
                    services.AddHostedService<Performer>();
                })
                .UseConsoleLifetime()
                .Build()
                .RunAsync();
        }

   }
}