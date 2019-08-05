using System.Threading;
using System.Threading.Tasks;
using Accounts.Application;
using Accounts.Ports.Commands;
using Accounts.Ports.Repositories;
using Paramore.Brighter;

namespace Accounts.Ports.Handlers
{
    public class AddNewAccountHandlerAsync : RequestHandlerAsync<AddNewAccountCommand>
    {
        private readonly IAccountRepositoryAsync _accountRepository;

        public AddNewAccountHandlerAsync(IAccountRepositoryAsync accountRepository)
        {
            _accountRepository = accountRepository;
        }
        
        public override Task<AddNewAccountCommand> HandleAsync(AddNewAccountCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            _accountRepository.AddAsync(new Account(
                    command.Id,
                    command.Name,
                    command.Addresses,
                    command.ContactDetails,
                    command.CardDetails
                )
            );
            return base.HandleAsync(command, cancellationToken);
        }
    }
}