using System;
using Paramore.Brighter;

namespace Accounts.Ports.Commands
{
    public class DeleteExistingAccountCommand : Command
    {
        public Guid AccountId { get; }

        public DeleteExistingAccountCommand(Guid accountId) : base(Guid.NewGuid())
        {
            AccountId = accountId;
        }
    }
}