using System;
using System.Threading.Tasks;
using JustRooms.DirectBookingEventConsumer.ports.events;
using JustSaying.Messaging.MessageHandling;

namespace JustRooms.DirectBookingEventConsumer.ports.handlers
{
    public class RoomBookingMadeHandler : IHandlerAsync<GuestRoomBookingMade>
    {
        public Task<bool> Handle(GuestRoomBookingMade message)
        {
            var tcs = new TaskCompletionSource<bool>();
            Console.WriteLine("Hello World");
            tcs.SetResult(true);
            return tcs.Task;
        }
    }
}