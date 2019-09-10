using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CreditCardCore.Adapters.Data;
using CreditCardCore.Application;
using CreditCardCore.Ports.Events;
using CreditCardCore.Ports.Repositories;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;

namespace CreditCardCore.Ports.Handlers
{
    public class UpsertAccountEventHandler : RequestHandler<UpsertAccountEvent>
    {
        private readonly DbContextOptions<CardDetailsContext> _options;
 
        public UpsertAccountEventHandler(DbContextOptions<CardDetailsContext> options)
        {
            _options = options;
        }

        public override UpsertAccountEvent Handle(UpsertAccountEvent @event)
        {
            //TODO: Brighter not handling async handlers yet, due to ordering concerns.
            HandleAsync(@event).GetAwaiter().GetResult();
            return base.Handle(@event);
        }

        public async Task HandleAsync(UpsertAccountEvent @event, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = new CardDetailsContext(_options))
            {
                var billingAddress = @event.Addresses.FirstOrDefault(addr => addr.AddressType == "Billing");
                if (billingAddress == null)
                {
                    throw new ArgumentException("Must have a billing address", "@event");
                }

                var repository = new AccountCardDetailsRepositoryAsync(new EFUnitOfWork(uow));

                var cardDetails = new AccountCardDetails(
                    accountId: Guid.Parse(@event.AccountId),
                    name: @event.Name.FirstName + " " + @event.Name.LastName,
                    cardNumber: @event.CardDetails.CardNumber,
                    cardSecurityCode: @event.CardDetails.CardSecurityCode,
                    firstLineOfAddress: billingAddress.FistLineOfAddress,
                    zipCode: billingAddress.ZipCode
                );

                await repository.UpsertAsync(cardDetails);
            }
        }
    }
}