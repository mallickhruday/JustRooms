using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CreditCardsAccountStreamReader.Application;
using CreditCardsAccountStreamReader.Ports.Events;
using CreditCardsAccountStreamReader.Ports.Repositories;
using Paramore.Brighter;

namespace CreditCardsAccountStreamReader.Ports.Handlers
{
    public class UpsertAccountEventHandler : RequestHandler<UpsertAccountEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpsertAccountEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override UpsertAccountEvent Handle(UpsertAccountEvent @event)
        {
            //TODO: Brighter not handling async handlers yet, due to ordering concerns.
            HandleAsync(@event).GetAwaiter().GetResult();
            return base.Handle(@event);
        }

        public async Task HandleAsync(UpsertAccountEvent @event, CancellationToken cancellationToken = new CancellationToken())
        {
            var billingAddress = @event.Addresses.FirstOrDefault(addr => addr.AddressType == "Billing");
            if (billingAddress == null)
            {
                throw new ArgumentException("Must have a billing address", "@event");
            }
            
            var repository = new AccountCardDetailsRepositoryAsync(_unitOfWork);

            var cardDetails = new AccountCardDetails(
                accountId: @event.AccountId,
                name: @event.Name.FirstName + " " + @event.Name.LastName,
                cardNumber: @event.CardDetails.CardNumber,
                cardSecurityCode: @event.CardDetails.CardSecurityCode,
                firstLineOfAddress: billingAddress.FistLineOfAddress,
                zipCode: billingAddress.ZipCode
            );
            cardDetails.CurrentVersion = @event.Version;
            
            await repository.UpsertAsync(cardDetails);
        }
    }
}