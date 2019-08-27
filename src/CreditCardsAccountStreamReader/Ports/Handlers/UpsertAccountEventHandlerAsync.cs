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
    public class UpsertAccountEventHandlerAsync : RequestHandlerAsync<UpsertAccountEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpsertAccountEventHandlerAsync(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<UpsertAccountEvent> HandleAsync(UpsertAccountEvent @event, CancellationToken cancellationToken = new CancellationToken())
        {
            var billingAddress = @event.Addresses.FirstOrDefault(addr => addr.AddressType == "Billing");
            if (billingAddress == null)
            {
                throw new ArgumentException("Must have a billing address", "@event");
            }
            
            var repository = new AccountCardDetailsRepositoryAsync(_unitOfWork);

            await repository.UpsertAsync(new AccountCardDetails(
                accountId: @event.AccountId,
                name: @event.Name.FirstName + " " + @event.Name.LastName,
                cardNumber: @event.CardDetails.CardNumber,
                cardSecurityCode: @event.CardDetails.CardSecurityCode,
                firstLineOfAddress: billingAddress.FistLineOfAddress,
                zipCode: billingAddress.ZipCode
            ));
                
            
            return await base.HandleAsync(@event, cancellationToken);
        }
    }
}