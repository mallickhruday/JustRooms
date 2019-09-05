using System;
using Paramore.Brighter;

namespace Accounts.Ports.Commands
{
    /// <summary>
    /// Delete an existing guest
    /// </summary>
    public class DeleteExistingAccountCommand : Command
    {
        /// <summary>
        /// The guest account to delete
        /// </summary>
        public Guid AccountId { get; }

        /// <summary>
        /// Construct a command to delete a guest account
        /// </summary>
        /// <param name="accountId">The guest account to delete</param>
        public DeleteExistingAccountCommand(Guid accountId) : base(Guid.NewGuid())
        {
            AccountId = accountId;
        }
    }
}