using System;
using System.Threading.Tasks;
using JustRooms.DirectBookingEventConsumer.ports.events;
using JustSaying.Messaging.MessageHandling;

namespace JustRooms.DirectBookingEventConsumer.ports.handlers
{
    public class GreetingHandler : IHandlerAsync<RoomBookingMade>
    {
        public Task<bool> Handle(RoomBookingMade message)
        {
            var tcs = new TaskCompletionSource<bool>();
            Console.WriteLine("Hello World");
            tcs.SetResult(true);
            return tcs.Task;
        }
    }
}