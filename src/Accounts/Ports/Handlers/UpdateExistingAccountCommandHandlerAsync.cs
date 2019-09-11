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

                        var account = await accountRepositoryAsync.GetAsync(command.AccountId);

                        account.Name = new Name(account, command.Name.FirstName, command.Name.LastName);
                        account.ContactDetails = new ContactDetails(account, command.ContactDetails.Email, command.ContactDetails.TelephoneNumber);
                        account.CardDetails = new CardDetails(account, command.CardDetails.CardNumber, command.CardDetails.CardSecurityCode);
                        account.Addresses = command.Addresses;
     
                        await accountRepositoryAsync.UpdateAsync(account, aggregateLock, cancellationToken);
                        
                        eventId = await _commandProcessor.DepositPostAsync(new AccountEvent
                        {
                            Id = Guid.NewGuid(),
                            AccountId = account.AccountId.ToString(),
                            Addresses = account.Addresses.Select(
                                addr => new AddressEvent
                                {
                                    AddressType = addr.AddressType.ToString(),
                                    FistLineOfAddress = addr.FistLineOfAddress,
                                    State = addr.State,
                                    ZipCode = addr.ZipCode
                                }).ToList(),
                            Name = new NameEvent {FirstName = account.Name.FirstName, LastName = account.Name.LastName},
                            CardDetails = new CardDetailsEvent
                            {
                                CardNumber = account.CardDetails.CardNumber,
                                CardSecurityCode = account.CardDetails.CardSecurityCode
                            },
                            ContactDetails = new ContactDetailsEvent
                            {
                                Email = account.ContactDetails.Email,
                                TelephoneNumber = account.ContactDetails.TelephoneNumber
                            }
                        }, false, cancellationToken);
                        
                        trans.Commit();
                    }
                    finally
                    {
                        if (aggregateLock != null)
                            await aggregateLock.ReleaseAsync(cancellationToken);
                    }
                }
            }
            
            _commandProcessor.ClearOutboxAsync(new []{eventId});

            return await base.HandleAsync(command, cancellationToken);
            
        }
    }
}