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
    public class DeleteExistingAccountCommandHandlerAsync : RequestHandlerAsync<DeleteExistingAccountCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteExistingAccountCommandHandlerAsync(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        [RequestLogging(step:0, HandlerTiming.Before)]
        [UsePolicy(Policies.Catalog.DynamoDbAccess, step: 0)]
        public override async Task<DeleteExistingAccountCommand> HandleAsync(DeleteExistingAccountCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            var repo = new AccountRepositoryAsync(_unitOfWork);

            await repo.DeleteAsync(command.AccountId);
            
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}