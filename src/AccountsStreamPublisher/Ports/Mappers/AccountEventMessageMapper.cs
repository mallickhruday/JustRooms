using AccountsTransferWorker.Ports.Events;
using Newtonsoft.Json;
using Paramore.Brighter;

namespace AccountsTransferWorker.Ports.Mappers
{
    public class AccountEventMessageMapper : IAmAMessageMapper<AccountEvent>
    {
        public Message MapToMessage(AccountEvent request)
        {
            var header = new MessageHeader(messageId: request.Id, topic: "account.event", messageType: MessageType.MT_EVENT);
            var body = new MessageBody(JsonConvert.SerializeObject(request));
            var message = new Message(header, body);
            return message;
        }

        public AccountEvent MapToRequest(Message message)
        {
            throw new System.NotImplementedException();
        }
    }
}