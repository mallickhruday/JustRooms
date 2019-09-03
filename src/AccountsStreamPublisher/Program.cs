using System;
using System.IO;
using System.Threading.Tasks;
using AccountsTransferWorker.Adapters.Data;
using AccountsTransferWorker.Adapters.Factories;
using AccountsTransferWorker.Ports;
using AccountsTransferWorker.Ports.Events;
using AccountsTransferWorker.Ports.Mappers;
using AccountsTransferWorker.Ports.Streams;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
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
        static async Task<int> Main(string[] args)
        {
            var host = BuildHost();
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
                    configurationBuilder.AddEnvironmentVariables(prefix: "ASP_");
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
                        .MapperRegistryFromAssemblies(typeof(AccountEventMessageMapper).Assembly);

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
                    
                    var translatorRegistry = new RecordTranslatorRegistry(new TranslatorFactory());
                    translatorRegistry.Add(typeof(AccountEvent), typeof(AccountFromRecordTranslator));

                    services.AddSingleton<RecordTranslatorRegistry>(translatorRegistry);
                    services.AddSingleton<IRecordProcessor<StreamRecord>, DynamoDbRecordProcessor>();
                    services.AddSingleton<IStreamReader, DynamoStreamReader>();       
                    services.AddHostedService<Pump>();
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