using System.Threading;
using System.Threading.Tasks;
using Accounts.Adapters.Data;
using Accounts.Application;
using Accounts.Ports.Commands;
using Paramore.Brighter;

namespace Accounts.Ports.Handlers
{
    public class UpdateExistingAccountCommandHandlerAsync : RequestHandlerAsync<UpdateExistingAccountCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateExistingAccountCommandHandlerAsync(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<UpdateExistingAccountCommand> HandleAsync(UpdateExistingAccountCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            var repo = new AccountRepositoryAsync(_unitOfWork);

           var newAccountVersion = new Account
            {
                AccountId = command.AccountId.ToString(),
                Name = command.Name,
                Addresses = command.Addresses,
                CardDetails = command.CardDetails,
                ContactDetails = command.ContactDetails,
            };

            await repo.UpdateAsync(newAccountVersion);
           
            return await base.HandleAsync(command, cancellationToken);
            
        }
    }
}