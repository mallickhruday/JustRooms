using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Adapters.Data;
using Accounts.Application;
using Accounts.Ports.Commands;
using Accounts.Ports.Events;
using Accounts.Ports.Repositories;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;
using Paramore.Brighter.Logging.Attributes;
using Paramore.Brighter.Policies.Attributes;

namespace Accounts.Ports.Handlers
{
    /// <summary>
    /// Handles request to update an existing guest account
    /// </summary>
    public class UpdateExistingAccountCommandHandlerAsync : RequestHandlerAsync<UpdateExistingAccountCommand>
    {
        private readonly DbContextOptions<AccountContext> _options;
        private readonly IAmACommandProcessor _commandProcessor;

        /// <summary>
        /// Constructs a handler for updating guest accounts
        /// </summary>
        /// <param name="options">The unit of work that lets us right to storage<</param>
        public UpdateExistingAccountCommandHandlerAsync(DbContextOptions<AccountContext> options, IAmACommandProcessor commandProcessor)
        {
            _options = options;
            _commandProcessor = commandProcessor;
        }

        /// <summary>
        /// Handle a request to update an existing account
        /// </summary>
        /// <param name="command">The details of the account to be updated and the new details</param>
        /// <param name="cancellationToken">A token that allows us to cancel an ongoing operation</param>
        /// <returns></returns>
        [RequestLoggingAsync(step:0, HandlerTiming.Before)]
        [UsePolicyAsync(Policies.Catalog.DynamoDbAccess, step: 0)]
        public override async Task<UpdateExistingAccountCommand> HandleAsync(UpdateExistingAccountCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            Guid eventId;
            using (var uow = new AccountContext(_options))
            {
                using (var trans = uow.Database.BeginTransaction())
                {
                    var accountRepositoryAsync = new AccountRepositoryAsync(new EFUnitOfWork(uow));

                    AggregateLock aggregateLock = null;
                    try
                    {
                        aggregateLock = await accountRepositoryAsync.LockAsync(command.AccountId.ToString(), command.LockBy,
                            cancellationToken);

                        var newAccountVersion = new Account
                        {
                            AccountId = command.AccountId,
                            Name = command.Name,
                            Addresses = command.Addresses,
                            CardDetails = command.CardDetails,
                            ContactDetails = command.ContactDetails,
                        };

                        await accountRepositoryAsync.UpdateAsync(newAccountVersion, aggregateLock);
                        
                        eventId =_commandProcessor.DepositPost(new AccountEvent
                        {
                            Id = Guid.NewGuid(),
                            AccountId = newAccountVersion.AccountId.ToString(),
                            Addresses = newAccountVersion.Addresses.Select(
                                addr => new AddressEvent
                                {
                                    AddressType = addr.AddressType.ToString(),
                                    FistLineOfAddress = addr.FistLineOfAddress,
                                    State = addr.State,
                                    ZipCode = addr.ZipCode
                                }).ToList(),
                            Name = new NameEvent {FirstName = newAccountVersion.Name.FirstName, LastName = newAccountVersion.Name.LastName},
                            CardDetails = new CardDetailsEvent
                            {
                                CardNumber = newAccountVersion.CardDetails.CardNumber,
                                CardSecurityCode = newAccountVersion.CardDetails.CardSecurityCode
                            },
                            ContactDetails = new ContactDetailsEvent
                            {
                                Email = newAccountVersion.ContactDetails.Email,
                                TelephoneNumber = newAccountVersion.ContactDetails.TelephoneNumber
                            },
                            Version = newAccountVersion.Version
                        });
                        
                        trans.Commit();
                    }
                    finally
                    {
                        if (aggregateLock != null)
                            await aggregateLock.ReleaseAsync(cancellationToken);
                    }
                }
            }
            
            _commandProcessor.ClearOutbox(eventId);

            return await base.HandleAsync(command, cancellationToken);
            
        }
    }
}