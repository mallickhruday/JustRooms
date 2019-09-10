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
using CreditCardCore.Ports.Handlers;
using CreditCardCore.Ports.Mappers;
using CreditCardCore.Ports.Repositories;
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
   }
}