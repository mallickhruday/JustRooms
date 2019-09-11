using System;
using Accounts.Ports.Events;
using Newtonsoft.Json;
using Paramore.Brighter;

namespace Accounts.Ports.Mappers
{
    /// <summary>
    /// Map accounts to a wire format for sending
    /// </summary>
    public class AccountEventMessageMapper : IAmAMessageMapper<AccountEvent>
    {
        /// <summary>
        /// Map to the Brighter wire format
        /// </summary>
        /// <param name="request">The request to map</param>
        /// <returns></returns>
        public Message MapToMessage(AccountEvent request)
        {
            var header = new MessageHeader(messageId: request.Id, topic: "account.event", messageType: MessageType.MT_EVENT);
            var body = new MessageBody(JsonConvert.SerializeObject(request));
            var message = new Message(header, body);
            return message;
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public AccountEvent MapToRequest(Message message)
        {
            throw new System.NotImplementedException();
        }
    }
}