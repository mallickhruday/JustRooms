using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace JustRooms.DirectBookingEventConsumer
{
    class Program
    {
        public static async Task Main()
        {
            Console.Title = "Direct Booking Event Consumer";

            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddJustSaying()
                })
                .UseConsoleLifetime()
                .Build();
        }

   }
}