using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using DirectBooking.adapters.data;
using DirectBooking.application;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Paramore.Brighter.DynamoDb.Extensions;
using Paramore.Brighter.Outbox.DynamoDB;
using Paramore.Brighter.Outbox.MySql;
using Polly;
using Serilog;
using Serilog.Events;

namespace DirectBooking
{
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
                var host = BuildHost(args);
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

        private static IWebHost BuildHost(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseUrls("http://*:5100")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .CaptureStartupErrors(true)
                .UseSetting(WebHostDefaults.DetailedErrorsKey, "true")
                .UseStartup<Startup>()
                .Build();

            using (var scope = host.Services.CreateScope())
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddEnvironmentVariables("ASPNETCORE_")
                    .Build();
                
                EnsureDatabaseCreated(scope.ServiceProvider);
                //CreateMessageTable(config["Database:MessageStore"], config["Database:MessageTableName"]);
            }

            return host;
        }
        
        private static void EnsureDatabaseCreated(IServiceProvider serviceProvider)
        {
            var contextOptions = serviceProvider.GetService<DbContextOptions<BookingContext>>();
            using (var context = new BookingContext(contextOptions))
            {
                context.Database.EnsureCreated();
            }
        }


        private static void CreateMessageTable(string dbConnectionString, string tableNameMessages)
        {
            try
            {
                using (var sqlConnection = new MySqlConnection(dbConnectionString))
                {
                    sqlConnection.Open();
                    using (var command = sqlConnection.CreateCommand())
                    {
                        command.CommandText = MySqlOutboxBuilder.GetDDL(tableNameMessages);
                        command.ExecuteScalar();
                    }
                }
            }
            catch (System.Exception e)
            {
                Log.Error($"Issue with creating MessageStore table, {e.Message}");
            }
        }
 
    }
}