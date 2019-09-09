using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using CreditCardCore.Adapters.Data;
using CreditCardCore.Application;
using CreditCardCore.Ports.Events;
using CreditCardCore.Ports.Repositories;
using Microsoft.Extensions.Configuration;
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

                    if (useLocalAwsServices )
                    { 
                        services.AddSingleton<IAmazonDynamoDB>(sp => CreateClient(hostContext.Configuration));
                    }
                    else
                    { 
                        services.AddAWSService<IAmazonDynamoDB>();
                    }
                    
                    services.AddSingleton<DynamoDbTableBuilder>();
                    services.AddSingleton<IUnitOfWork, DynamoDbUnitOfWork>();
         
                })
                .UseSerilog()
                .UseConsoleLifetime()
                .Build();

            return host;
        }
        
            private static IAmazonDynamoDB CreateClient(IConfiguration configuration)
            {
                var credentials = GetAwsCredentials(configuration);
                var serviceUrl = configuration.GetValue<string>("DynamoDb:LocalServiceUrl");
                var clientConfig = new AmazonDynamoDBConfig { ServiceURL = serviceUrl };
                return new AmazonDynamoDBClient(credentials, clientConfig);
            }
    
           private static BasicAWSCredentials GetAwsCredentials(IConfiguration configuration)
            {
                var accessKey = configuration.GetValue<string>("AWS_ACCESS_KEY_ID");
                var accessSecret = configuration.GetValue<string>("AWS_SECRET_ACCESS_KEY");
                var credentials = new BasicAWSCredentials(accessKey, accessSecret);
                return credentials;
            }
     
    }
    
}