using JustRooms.DirectBookingEventConsumer.ports.handlers;
using JustSaying;
using JustSaying.Messaging.MessageHandling;

namespace JustRooms.DirectBookingEventConsumer.adapters.di
{
    public class HandlerResolver : IHandlerResolver
    {
        public IHandlerAsync<T> ResolveHandler<T>(HandlerResolutionContext context)
        {
            if (context.QueueName == Globals.BOOKING_QUEUE_NAME)
            {
                return (IHandlerAsync<T>) new RoomBookingMadeHandler();
            }
            else
            {
                throw new HandlerNotRegisteredWithContainerException($"Could not find registered handler for {Globals.BOOKING_QUEUE_NAME}");
            }
        }
    }
}