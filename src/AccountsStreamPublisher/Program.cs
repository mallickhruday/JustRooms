using System;
using System.IO;
using System.Threading.Tasks;
using AccountsTransferWorker.Adapters.Data;
using AccountsTransferWorker.Adapters.Factories;
using AccountsTransferWorker.Ports;
using AccountsTransferWorker.Ports.Events;
using AccountsTransferWorker.Ports.Streams;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Paramore.Brighter.Extensions.DependencyInjection;
using Paramore.Brighter.MessagingGateway.Kafka;
using Polly;
using Polly.Registry;
using Serilog;

namespace AccountsTransferWorker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var app = new CommandLineApplication();

            app.HelpOption("-h|--help");
            var optionEnvironment = app.Option(
                "-e|--environment <ENVIRONMENT>", "Environment: Development, Staging, Production", 
                CommandOptionType.SingleValue
                );

            app.OnExecuteAsync(async cancellationToken =>
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
                        var gatewayConnection = new KafkaMessagingGatewayConfiguration 
                        {
                            Name = "paramore.brighter.accounttransfer",
                            BootStrapServers = new[] {"localhost:9092"}
                        };
                        
                        var retryPolicy = Policy.Handle<Exception>().WaitAndRetry(new[] { TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(150) });
                        var circuitBreakerPolicy = Policy.Handle<Exception>().CircuitBreaker(1, TimeSpan.FromMilliseconds(500));
                        var retryPolicyAsync = Policy.Handle<Exception>().WaitAndRetryAsync(new[] { TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(150) });
                        var circuitBreakerPolicyAsync = Policy.Handle<Exception>().CircuitBreakerAsync(1, TimeSpan.FromMilliseconds(500));
                        var policyRegistry = new PolicyRegistry()
                        {
                            { CommandProcessor.RETRYPOLICY, retryPolicy },
                            { CommandProcessor.CIRCUITBREAKER, circuitBreakerPolicy },
                            { CommandProcessor.RETRYPOLICYASYNC, retryPolicyAsync },
                            { CommandProcessor.CIRCUITBREAKERASYNC, circuitBreakerPolicyAsync }
                        };

                        services.AddSingleton<IReadOnlyPolicyRegistry<string>>(policyRegistry);
                        var producer = new KafkaMessageProducerFactory(gatewayConnection).Create();
                        services
                            .AddBrighter(options =>
                            {
                                options.PolicyRegistry = policyRegistry;
                                options.BrighterMessaging = new BrighterMessaging(new InMemoryMessageStore(), producer);
                            })
                            .AutoFromAssemblies();

                        var dynamoDbConfig = hostContext.Configuration.GetSection("DynamoDb");
                        var runLocalDynamoDb = dynamoDbConfig.GetValue<bool>("LocalMode");

                        if (runLocalDynamoDb)
                        { 
                            services.AddSingleton<IAmazonDynamoDB>(sp =>
                            { 
                                var clientConfig = new AmazonDynamoDBConfig
                                {
                                    ServiceURL = dynamoDbConfig.GetValue<string>("LocalServiceUrl")
                                };
                                return new AmazonDynamoDBClient(clientConfig);
                            });
                            services.AddSingleton<IAmazonDynamoDBStreams>(sp =>
                            { 
                                var clientConfig = new AmazonDynamoDBStreamsConfig
                                {
                                    ServiceURL = dynamoDbConfig.GetValue<string>("LocalServiceUrl")
                                };
                                return new AmazonDynamoDBStreamsClient(clientConfig);
                            });
                        }
                        else
                        { 
                            services.AddAWSService<IAmazonDynamoDB>();
                            services.AddAWSService<IAmazonDynamoDBStreams>();
                        }
                        
                        var translatorRegistry = new RecordTranslatorRegistry(new TranslatorFactory());
                        translatorRegistry.Add(typeof(AccountEvent), typeof(AccountFromRecordTranslator));

                        services.AddSingleton<RecordTranslatorRegistry>(translatorRegistry);
                        services.AddSingleton<IRecordProcessor<StreamRecord>, DynamoDbRecordProcessor>();
                        services.AddSingleton<IStreamReader, DynamoStreamReader>();       
                        services.AddHostedService<Pump>();
                    })
                    .UseSerilog()
                    .UseConsoleLifetime()
                    .Build()
                    .RunAsync(cancellationToken);
                });

            await app.ExecuteAsync(args);
        }
    }
}