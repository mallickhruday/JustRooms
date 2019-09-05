using System.Threading;
using System.Threading.Tasks;
using Accounts.Application;
using Accounts.Ports.Commands;
using Accounts.Ports.Repositories;
using Paramore.Brighter;
using Paramore.Brighter.Logging.Attributes;
using Paramore.Brighter.Policies.Attributes;

namespace Accounts.Ports.Handlers
{
    /// <summary>
    /// Handler for add new Accounts
    /// </summary>
    public class AddNewAccountHandlerAsync : RequestHandlerAsync<AddNewAccountCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Construct an add new Account handler
        /// </summary>
        /// <param name="unitOfWork">A unit of work, used to talk to the storage layer</param>
        public AddNewAccountHandlerAsync(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        /// <summary>
        /// Handle a request to add a guest account
        /// </summary>
        /// <param name="command">The request to add a guest account</param>
        /// <param name="cancellationToken">Cancel an ongoing operatioin</param>
        /// <returns>Passes the command to the next handler in the chain</returns>
        [RequestLoggingAsync(step:0, HandlerTiming.Before)]
        [UsePolicyAsync(Policies.Catalog.DynamoDbAccess, step: 0)]
        public override async Task<AddNewAccountCommand> HandleAsync(AddNewAccountCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            var accountRepository = new AccountRepositoryAsync(_unitOfWork);
            await accountRepository.AddAsync(new Account(
                    command.Id,
                    command.Name,
                    command.Addresses,
                    command.ContactDetails,
                    command.CardDetails
                )
            );
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}