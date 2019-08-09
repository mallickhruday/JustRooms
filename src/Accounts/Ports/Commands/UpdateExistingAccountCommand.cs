using System;
using System.Collections.Generic;
using Accounts.Application;
using Paramore.Brighter;

namespace Accounts.Ports.Commands
{
    public class UpdateExistingAccountCommand : Command
    {
        public UpdateExistingAccountCommand() : base(Guid.NewGuid()) {}
        
        public Guid AccountId { get; set; }
        public Name Name { get; set; }
        public List<Address> Addresses { get; set; }
        public ContactDetails ContactDetails { get; set; }
        public CardDetails CardDetails { get; set; }
    }
}