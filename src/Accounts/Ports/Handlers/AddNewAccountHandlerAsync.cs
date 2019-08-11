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
    public class AddNewAccountHandlerAsync : RequestHandlerAsync<AddNewAccountCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddNewAccountHandlerAsync(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        [RequestLogging(step:0, HandlerTiming.Before)]
        [UsePolicy(Policies.Catalog.DynamoDbAccess, step: 0)]
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