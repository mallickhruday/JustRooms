using CreditCardCore.Ports.Events;
using Newtonsoft.Json;
using Paramore.Brighter;

namespace CreditCardCore.Ports.Mappers
{
    public class GuestRoomBookingMadeMapper : IAmAMessageMapper<GuestRoomBookingMade>
    {
        public Message MapToMessage(GuestRoomBookingMade request)
        {
            throw new System.NotImplementedException();
       }

        public GuestRoomBookingMade MapToRequest(Message message)
        {
            return JsonConvert.DeserializeObject<GuestRoomBookingMade>(message.Body.Value);
        }
    }
}