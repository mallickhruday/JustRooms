using System;
using System.Collections.Generic;
using Paramore.Brighter;

namespace AccountsTransferWorker.Ports.Events
{
    public class AccountEvent : Event
    {
        public string AccountId { get; set; }
        public Name Name { get; set; }
        public List<Address> Addresses { get; set; }
        public ContactDetails ContactDetails { get; set; }
        public CardDetails CardDetails { get; set; }
        public int Version { get; set; }

        public AccountEvent() : base(Guid.NewGuid()) {}
    }
}