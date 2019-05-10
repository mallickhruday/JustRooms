using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime.CredentialManagement;
using JustRooms.GuestDirectBooking.adapters.bus;
using JustRooms.GuestDirectBooking.ports.events;
using JustSaying;
using JustSaying.AwsTools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace JustRooms.GuestDirectBooking
{
    class Program
    {
        
        static async Task Main(string[] args)
        {
            if (new CredentialProfileStoreChain().TryGetAWSCredentials("default", out var awsCredentials))
            {
                await new HostBuilder()
                    .ConfigureLogging(loggingBuilder => loggingBuilder.AddConsole())
                    .ConfigureServices((hostContext, services) =>
                    {
                        var configuration = hostContext.Configuration;
                        services.AddJustSaying((config) =>
                        {
                            config.Client(x =>
                            {
                                x.WithClientFactory(() => new DefaultAwsClientFactory(awsCredentials));
                            });
                            config.Messaging(x => { x.WithRegions(RegionEndpoint.EUWest1.SystemName); });
                            config.Publications(x => { x.WithTopic<GuestRoomBookingMade>(); });
                        });
                        services.AddHostedService<Producer>();
                    })
                    .UseConsoleLifetime()
                    .Build()
                    .RunAsync();
 


            }
        }
    }
}
