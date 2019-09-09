using CreditCardCore.Ports.Events;
using Newtonsoft.Json;
using Paramore.Brighter;

namespace CreditCardCore.Ports.Mappers
{
    public class AccountEventMessageMapper : IAmAMessageMapper<UpsertAccountEvent>
    {
        public Message MapToMessage(UpsertAccountEvent request)
        {
            throw new System.NotImplementedException();
        }

        public UpsertAccountEvent MapToRequest(Message message)
        {
            //TODO: This could really be a default generic mapper, as we just serialize the body
            return JsonConvert.DeserializeObject<UpsertAccountEvent>(message.Body.Value);
        }
    }
}