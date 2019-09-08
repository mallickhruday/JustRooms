using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Accounts.Application;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Paramore.Brighter.DynamoDb.Extensions;
using Paramore.Brighter.Outbox.DynamoDB;
using Serilog;
using Serilog.Events;

namespace Accounts
{
    /// <summary>
    /// Manages account information
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry point
        /// </summary>
        /// <param name="args">Run time arguments</param>
        /// <returns>0 indicates a successful exit, 1 an error</returns>
        public static async Task<int> Main(string[] args)
        {
             Environment.SetEnvironmentVariable("AWS_ENABLE_ENDPOINT_DISCOVERY", "false");
             
             Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Information("Starting web host");
                var host = await BuildHost(args);
                host.Run();
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

        private static async Task<IWebHost> BuildHost(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseUrls("http://*:5000")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .CaptureStartupErrors(true)
                .UseSetting(WebHostDefaults.DetailedErrorsKey, "true")
                .UseStartup<Startup>()
                .Build();
            
            using (var scope = host.Services.CreateScope())
            {
                var dbBuilder = scope.ServiceProvider.GetService<DynamoDbTableBuilder>();
                var hasTables = await dbBuilder.HasTables(new string[]{"Accounts"});
                if (!hasTables.Item1)
                {
                    await dbBuilder.Build(
                        new DynamoDbTableFactory()
                            .GenerateCreateTableMapper<Account>(
                                new DynamoDbCreateProvisionedThroughput(
                                    new ProvisionedThroughput(readCapacityUnits: 10, writeCapacityUnits:10),
                                    new Dictionary<string, ProvisionedThroughput>()
                                ),
                                billingMode: BillingMode.PROVISIONED,
                                sseSpecification: new SSESpecification{Enabled = true},
                                streamSpecification: new StreamSpecification
                                {
                                    StreamEnabled = true,
                                    StreamViewType = StreamViewType.NEW_IMAGE
                                })
                        );
                    await dbBuilder.EnsureTablesReady(new string[] {"Accounts"}, TableStatus.ACTIVE);
                }
            }

            return host;
        }
    }
}