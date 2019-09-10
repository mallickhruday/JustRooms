using System;
using System.Collections.Generic;
using Paramore.Brighter;

namespace Accounts.Ports.Events
{
    public class AccountEvent : Event
    {
        public string AccountId { get; set; }
        public NameEvent Name { get; set; }
        public List<AddressEvent> Addresses { get; set; }
        public ContactDetailsEvent ContactDetails { get; set; }
        public CardDetailsEvent CardDetails { get; set; }
        public byte[] Version { get; set; }

        public AccountEvent() : base(Guid.NewGuid()) {}
    }
}