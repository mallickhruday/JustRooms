using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using CreditCardCore.Application;
using CreditCardCore.Ports.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Paramore.Brighter.DynamoDb.Extensions;
using Paramore.Brighter.Extensions.DependencyInjection;
using Paramore.Brighter.MessagingGateway.RMQ;
using Paramore.Brighter.Outbox.DynamoDB;
using Paramore.Brighter.ServiceActivator.Extensions.DependencyInjection;
using Paramore.Brighter.ServiceActivator.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;

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
                var dbBuilder = scope.ServiceProvider.GetService<DynamoDbTableBuilder>();
                var hasTables = await dbBuilder.HasTables(new string[] {"CardDetails"});
                if (!hasTables.exist)
                {
                    await dbBuilder.Build(
                        new DynamoDbTableFactory()
                            .GenerateCreateTableMapper<AccountCardDetails>(
                                new DynamoDbCreateProvisionedThroughput(
                                    new ProvisionedThroughput(readCapacityUnits: 10, writeCapacityUnits: 10),
                                    new Dictionary<string, ProvisionedThroughput>()
                                ),
                                billingMode: BillingMode.PAY_PER_REQUEST,
                                sseSpecification: new SSESpecification {Enabled = true}
                            )
                    );
                    await dbBuilder.EnsureTablesReady(new string[] {"CardDetails"}, TableStatus.ACTIVE);
                }
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
                        Exchange = new Exchange("paramore.brighter.exchange")
                    };

                    var rmqMessageConsumerFactory = new RmqMessageConsumerFactory(rmqConnection);

                    services.AddServiceActivator(options =>
                    {
                        options.Connections = connections;
                        options.ChannelFactory = new ChannelFactory(rmqMessageConsumerFactory);
                        options.BrighterMessaging = new BrighterMessaging(new InMemoryOutbox(),
                            new RmqMessageProducer(rmqConnection));
                    }).AutoFromAssemblies();

                    services.AddSingleton<ILoggerFactory>(x => new SerilogLoggerFactory());
                    services.AddHostedService<ServiceActivatorHostedService>();
                })
                .UseConsoleLifetime()
                .Build();

            return host;
        }
    }
}