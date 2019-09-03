using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using CreditCardsAccountStreamReader.Adapters.Data;
using CreditCardsAccountStreamReader.Application;
using CreditCardsAccountStreamReader.Ports.Events;
using CreditCardsAccountStreamReader.Ports.Handlers;
using CreditCardsAccountStreamReader.Ports.Mappers;
using CreditCardsAccountStreamReader.Ports.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Paramore.Brighter.DynamoDb.Extensions;
using Paramore.Brighter.MessagingGateway.Kafka;
using Paramore.Brighter.Outbox.DynamoDB;
using Paramore.Brighter.ServiceActivator.Extensions.DependencyInjection;
using Paramore.Brighter.ServiceActivator.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace CreditCardsAccountStreamReader
{
    class Program
    {
        static async Task<int>  Main(string[] args)
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
            return new HostBuilder()
                .ConfigureLogging(loggingBuilder => loggingBuilder.AddConsole())
                .ConfigureHostConfiguration((configurationBuilder) =>
                {
                    configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
                    configurationBuilder.AddEnvironmentVariables(prefix: "ASR_");
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var useLocalAwsServices = hostContext.Configuration.GetValue<bool>("AWS:UseLocalServices");

                    if (useLocalAwsServices )
                    { 
                        services.AddSingleton<IAmazonDynamoDB>(sp => CreateClient(hostContext.Configuration));
                        services.AddSingleton<IAmazonDynamoDBStreams>(sp => CreateStreamClient(hostContext.Configuration));
                    }
                    else
                    { 
                        services.AddAWSService<IAmazonDynamoDB>();
                        services.AddAWSService<IAmazonDynamoDBStreams>();
                    }
 
                    var connections = new Connection[]
                    {
                        new Connection<UpsertAccountEvent>(
                            new ConnectionName("credit.card.account.stream"),
                            new ChannelName("account.event"),
                            new RoutingKey("account.event"),
                            timeoutInMilliseconds: 200,
                            isDurable: true,
                            highAvailability: true)
                    };

                    var gatewayConfiguration = new KafkaMessagingGatewayConfiguration
                    {
                        Name = "paramore.brighter.accounttransfer",
                        BootStrapServers = new[] {"localhost:9092"}
                    };

                    var messageConsumerFactory = new KafkaMessageConsumerFactory(gatewayConfiguration);

                  services.AddScoped<DynamoDbTableBuilder>();
                  services.AddScoped<IUnitOfWork, DynamoDbUnitOfWork>();
                  services.AddServiceActivator(options =>
                    {
                        options.Connections = connections;
                        options.ChannelFactory = new ChannelFactory(messageConsumerFactory);
                    })
                    .HandlersFromAssemblies(typeof(UpsertAccountEventHandler).Assembly)
                    .MapperRegistryFromAssemblies(typeof(AccountEventMessageMapper).Assembly);

                    services.AddSingleton<ILoggerFactory>(x => new SerilogLoggerFactory());
                    services.AddHostedService<ServiceActivatorHostedService>();
                })
                .UseSerilog()
                .UseConsoleLifetime()
                .Build();
        }
        
        private static IAmazonDynamoDB CreateClient(IConfiguration configuration)
        {
            var credentials = GetAwsCredentials(configuration);
            var serviceUrl = configuration.GetValue<string>("DynamoDb:LocalServiceUrl");
            var clientConfig = new AmazonDynamoDBConfig { ServiceURL = serviceUrl };
            return new AmazonDynamoDBClient(credentials, clientConfig);
        }

        private static IAmazonDynamoDBStreams CreateStreamClient(IConfiguration configuration)
        {
            var credentials = GetAwsCredentials(configuration);
            var serviceUrl = configuration.GetValue<string>("DynamoDb:LocalServiceUrl");
            var clientConfig = new AmazonDynamoDBStreamsConfig { ServiceURL = serviceUrl };
            return new AmazonDynamoDBStreamsClient(credentials, clientConfig);
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