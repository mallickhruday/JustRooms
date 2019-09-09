using DirectBooking.ports.events;
using Newtonsoft.Json;
using Paramore.Brighter;

namespace DirectBooking.ports.mappers
{
    public class GuestRoomBookingMadeMapper : IAmAMessageMapper<GuestRoomBookingMade>
    {
        public Message MapToMessage(GuestRoomBookingMade request)
        {
            var header = new MessageHeader(messageId: request.Id, topic: "booking.event", messageType: MessageType.MT_EVENT);
            var body = new MessageBody(JsonConvert.SerializeObject(request));
            var message = new Message(header, body);
            return message;
        }

        public GuestRoomBookingMade MapToRequest(Message message)
        {
            throw new System.NotImplementedException();
        }
    }
}