using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Accounts
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = await BuildHost(args);
            host.Run();
        }

        private static async Task<IWebHost> BuildHost(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
            
            using (var scope = host.Services.CreateScope())
            {
                //TODO: Add builder var dbBuilder = scope.ServiceProvider.GetService<DbBuilder>();
                //await dbBuilder.Create();
            }

            return host;
        }
    }
}