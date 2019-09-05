using System.Threading;
using System.Threading.Tasks;
using Accounts.Adapters.Data;
using Accounts.Ports.Commands;
using Accounts.Ports.Repositories;
using Paramore.Brighter;
using Paramore.Brighter.Logging.Attributes;
using Paramore.Brighter.Policies.Attributes;

namespace Accounts.Ports.Handlers
{
    /// <summary>
    /// Handles deleting an existing account version
    /// </summary>
    public class DeleteExistingAccountCommandHandlerAsync : RequestHandlerAsync<DeleteExistingAccountCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructs a delete account handler
        /// </summary>
        /// <param name="unitOfWork">The storage we intend to delete from</param>
        public DeleteExistingAccountCommandHandlerAsync(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        /// <summary>
        /// Handles deleting a guest account (version)
        /// </summary>
        /// <param name="command">The command for the account (version) to delete</param>
        /// <param name="cancellationToken">A token that allows cancelling the ongoing operation</param>
        /// <returns>Passes the command to the next handler in the chain</returns>
        [RequestLoggingAsync(step:0, HandlerTiming.Before)]
        [UsePolicyAsync(Policies.Catalog.DynamoDbAccess, step: 0)]
        public override async Task<DeleteExistingAccountCommand> HandleAsync(DeleteExistingAccountCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            var repo = new AccountRepositoryAsync(_unitOfWork);

            await repo.DeleteAsync(command.AccountId);
            
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}