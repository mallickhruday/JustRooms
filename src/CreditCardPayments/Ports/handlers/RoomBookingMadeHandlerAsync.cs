using JustRooms.DirectBookingEventConsumer.Ports.events;
using Paramore.Brighter;

namespace JustRooms.DirectBookingEventConsumer.Ports.handlers
{
    public class RoomBookingMadeHandlerAsync : RequestHandler<GuestRoomBookingMade>
    {
        public override GuestRoomBookingMade Handle(GuestRoomBookingMade command)
        {
            return base.Handle(command);
        }
   }
}