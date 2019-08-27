using System.Threading;
using System.Threading.Tasks;
using Accounts.Adapters.Data;
using Accounts.Application;
using Accounts.Ports.Commands;
using Accounts.Ports.Repositories;
using Paramore.Brighter;
using Paramore.Brighter.Logging.Attributes;
using Paramore.Brighter.Policies.Attributes;

namespace Accounts.Ports.Handlers
{
    public class UpdateExistingAccountCommandHandlerAsync : RequestHandlerAsync<UpdateExistingAccountCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateExistingAccountCommandHandlerAsync(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [RequestLoggingAsync(step:0, HandlerTiming.Before)]
        [UsePolicyAsync(Policies.Catalog.DynamoDbAccess, step: 0)]
        public override async Task<UpdateExistingAccountCommand> HandleAsync(UpdateExistingAccountCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
           var repo = new AccountRepositoryAsync(_unitOfWork);

           AggregateLock aggregateLock = null;
           try
           {
               aggregateLock = await repo.LockAsync(command.AccountId.ToString(), command.LockBy, cancellationToken);

               var newAccountVersion = new Account
               {
                    AccountId = command.AccountId.ToString(),
                    Name = command.Name,
                    Addresses = command.Addresses,
                    CardDetails = command.CardDetails,
                    ContactDetails = command.ContactDetails,
               };
               
               await repo.UpdateAsync(newAccountVersion, aggregateLock);
           }
           finally
           {
               if (aggregateLock != null)
                   await aggregateLock.ReleaseAsync(cancellationToken);
           }

           return await base.HandleAsync(command, cancellationToken);
            
        }
    }
}