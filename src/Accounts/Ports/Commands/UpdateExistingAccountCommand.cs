using System;
using System.Collections.Generic;
using Accounts.Application;
using Paramore.Brighter;

namespace Accounts.Ports.Commands
{
    public class UpdateExistingAccountCommand : Command
    {
        public UpdateExistingAccountCommand(Guid accountId, Name name, List<Address> addresses, ContactDetails contactDetails, CardDetails cardDetails) 
            : base(Guid.NewGuid())
        {
            AccountId = accountId;
            Addresses = addresses;
            ContactDetails = contactDetails;
            CardDetails = cardDetails;
        }
        
        public Guid AccountId { get; set; }
        public Name Name { get; set; }
        public List<Address> Addresses { get; set; }
        public ContactDetails ContactDetails { get; set; }
        public CardDetails CardDetails { get; set; }
        public string LockBy { get; set; }
    }
}