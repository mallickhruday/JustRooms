using System;
using System.IO;
using System.Threading.Tasks;
using CreditCardCore.Adapters.Data;
using CreditCardCore.Ports.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Paramore.Brighter.Extensions.DependencyInjection;
using Paramore.Brighter.MessagingGateway.RMQ;
using Paramore.Brighter.ServiceActivator.Extensions.DependencyInjection;
using Paramore.Brighter.ServiceActivator.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace JustRooms.DirectBookingEventConsumer
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            Environment.SetEnvironmentVariable("AWS_ENABLE_ENDPOINT_DISCOVERY", "false");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            var host = BuildHost();

            using (var scope = host.Services.CreateScope())
            {
            }

            try
            {
                await host.RunAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHost BuildHost()
        {
            var host = new HostBuilder()
                 .ConfigureLogging(loggingBuilder => loggingBuilder.AddConsole())
                 .ConfigureHostConfiguration((configurationBuilder) =>
                 {
                     configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
                     configurationBuilder.AddEnvironmentVariables(prefix: "CCP_");
                 })
                 .ConfigureServices((hostContext, services) =>

                {
                    var connections = new Connection[]
                    {
                        new Connection<GuestRoomBookingMade>(
                            new ConnectionName("credit.card.booking.event"),
                            new ChannelName("credit.card.booking.event"),
                            new RoutingKey("booking.event"),
                            timeoutInMilliseconds: 200,
                            isDurable: true,
                            highAvailability: true)
                    };

                    var rmqConnection = new RmqMessagingGatewayConnection
                    {
                        AmpqUri = new AmqpUriSpecification(new Uri("amqp://guest:guest@localhost:5672")),
                        Exchange = new Exchange("hotel.booking.exchange")
                    };

                    var rmqMessageConsumerFactory = new RmqMessageConsumerFactory(rmqConnection);

                    services.AddServiceActivator(options =>
                    {
                        options.Connections = connections;
                        options.ChannelFactory = new ChannelFactory(rmqMessageConsumerFactory);
                        options.BrighterMessaging = new BrighterMessaging(new InMemoryOutbox(),
                            new RmqMessageProducer(rmqConnection));
                    }).AutoFromAssemblies();

                    services.AddHostedService<ServiceActivatorHostedService>();
                    var useLocalAwsServices = hostContext.Configuration.GetValue<bool>("AWS:UseLocalServices");

                    services.AddDbContext<CardDetailsContext>(options =>
                        options.UseMySql(hostContext.Configuration["Database:Bookings"])); 

        
                })
                .UseSerilog()
                .UseConsoleLifetime()
                .Build();

            return host;
        }
    }
    
}