using System;
using System.IO;
using System.Threading.Tasks;
using CreditCardsAccountStreamReader.Ports.Events;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Paramore.Brighter.Extensions.DependencyInjection;
using Paramore.Brighter.MessagingGateway.Kafka;
using Paramore.Brighter.ServiceActivator.Extensions.DependencyInjection;
using Paramore.Brighter.ServiceActivator.Extensions.Hosting;
using Serilog;
using Serilog.Extensions.Logging;

namespace CreditCardsAccountStreamReader
{
    class Program
    {
        static async Task  Main(string[] args)
        {
            var app = new CommandLineApplication();

            app.HelpOption("-h|--help");
            var optionEnvironment = app.Option(
                "-e|--environment <ENVIRONMENT>", "Environment: Development, Staging, Production", 
                CommandOptionType.SingleValue
                );

            app.OnExecuteAsync(async cancellcationToken =>
            {
                 var environment = optionEnvironment.HasValue()
                    ? optionEnvironment.Value()
                    : "Development";
                
                await new HostBuilder()
                    .UseEnvironment(environment)
                    .ConfigureLogging(loggingBuilder => loggingBuilder.AddConsole())
                    .ConfigureAppConfiguration((hostContext, configApp) =>
                    {
                        configApp.SetBasePath(Directory.GetCurrentDirectory());
                        configApp.AddJsonFile("appsettings.json", optional: true);
                        configApp.AddJsonFile(
                            $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", 
                            optional: true);
                        configApp.AddEnvironmentVariables(prefix: "ATW_");
                    })
                    .ConfigureServices((hostContext, services) =>
                    {
                                           var connections = new Connection[]
                    {
                        new Connection<AccountEvent>(
                            new ConnectionName("paramore.example.greeting"),
                            new ChannelName("greeting.event"),
                            new RoutingKey("greeting.event"),
                            timeoutInMilliseconds: 200,
                            isDurable: true,
                            highAvailability: true)
                    };
                                           
                    var gatewayConfiguration = new KafkaMessagingGatewayConfiguration
                    {
                         Name = "paramore.brighter",
                         BootStrapServers = new[] { "localhost:9092" }
                    };

                    var messageConsumerFactory = new KafkaMessageConsumerFactory(gatewayConfiguration); 

                    services.AddServiceActivator(options =>
                    {
                        options.Connections = connections;
                        options.ChannelFactory = new ChannelFactory(messageConsumerFactory);
                    }).AutoFromAssemblies();

                    services.AddSingleton<ILoggerFactory>(x => new SerilogLoggerFactory());
                    services.AddHostedService<ServiceActivatorHostedService>();
  
                    })
                    .UseSerilog()
                    .UseConsoleLifetime()
                    .Build()
                    .RunAsync(cancellcationToken);
            });
        }
    }
}