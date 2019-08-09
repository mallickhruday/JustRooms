using System.Threading;
using System.Threading.Tasks;
using Accounts.Adapters.Data;
using Accounts.Ports.Commands;
using Paramore.Brighter;

namespace Accounts.Ports.Handlers
{
    public class DeleteExistingAccountCommandHandlerAsync : RequestHandlerAsync<DeleteExistingAccountCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteExistingAccountCommandHandlerAsync(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public override async Task<DeleteExistingAccountCommand> HandleAsync(DeleteExistingAccountCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            var repo = new AccountRepositoryAsync(_unitOfWork);

            await repo.DeleteAsync(command.AccountId);
            
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}