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
    /// Handler for add new Accounts
    /// </summary>
    public class AddNewAccountHandlerAsync : RequestHandlerAsync<AddNewAccountCommand>
    {
        private readonly DbContextOptions<AccountContext> _options;
        private readonly IAmACommandProcessor _commandProcessor;

        /// <summary>
        /// Construct an add new Account handler
        /// </summary>
        /// <param name="options">A unit of work, used to talk to the storage layer</param>
        public AddNewAccountHandlerAsync(DbContextOptions<AccountContext> options, IAmACommandProcessor commandProcessor)
        {
            _options = options;
            _commandProcessor = commandProcessor;
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
            Guid eventId;
            using (var uow = new AccountContext(_options))
            {
                using (var trans = uow.Database.BeginTransaction())
                {
                    var accountRepository = new AccountRepositoryAsync(new EFUnitOfWork(uow));
                    
                    var account = new Account();
                    account.AccountId = command.Id;
                    account.Name = new Name(account, command.Name.FirstName, command.Name.LastName);
                    account.ContactDetails = new ContactDetails(account, command.ContactDetails.Email, command.ContactDetails.TelephoneNumber);
                    account.CardDetails = new CardDetails(account, command.CardDetails.CardNumber, command.CardDetails.CardSecurityCode);
                    account.Addresses = command.Addresses;
                    
                    await accountRepository.AddAsync(account);

                    eventId =_commandProcessor.DepositPost(new AccountEvent
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
                        },
                        Version = account.Version
                    });
                    
                    trans.Commit();
                }
            }
            
            _commandProcessor.ClearOutbox(eventId);
            

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}