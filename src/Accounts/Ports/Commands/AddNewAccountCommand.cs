using System;
using System.Collections.Generic;
using Accounts.Application;
using Paramore.Brighter;

namespace Accounts.Ports.Commands
{
    public class AddNewAccountCommand : Command
    {
        public AddNewAccountCommand() : base(Guid.NewGuid()){}
        public Name Name { get; set; }
        public List<Address> Addresses { get; set; }
        public ContactDetails ContactDetails { get; set; }
        public CardDetails CardDetails { get; set; }
    }
}