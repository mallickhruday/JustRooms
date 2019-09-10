using System;
using CreditCardCore.Adapters.Data;
using CreditCardCore.Application;
using CreditCardCore.Ports.Events;
using CreditCardCore.Ports.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace CreditCardCore.Ports.Handlers
{
    public class RoomBookingMadeHandlerAsync : RequestHandler<GuestRoomBookingMade>
    {
        private readonly DbContextOptions<CardDetailsContext> _options;
        private readonly ILogger<RoomBookingMadeHandlerAsync> _logger;

        public RoomBookingMadeHandlerAsync(DbContextOptions<CardDetailsContext> options, ILogger<RoomBookingMadeHandlerAsync> logger)
        {
            _options = options;
            this._logger = logger;
        }
        public override GuestRoomBookingMade Handle(GuestRoomBookingMade @event)
        {
            AccountCardDetails cardDetails;
            using (var uow = new CardDetailsContext(_options))
            {
                var repo = new AccountCardDetailsRepositoryAsync(new EFUnitOfWork(uow));

                cardDetails = repo.GetAsync(Guid.Parse(@event.AccountId)).GetAwaiter().GetResult();

                if (cardDetails == null)
                {
                    _logger.LogError("Unable to find card details for account: {0}", @event.AccountId);
                    throw new InvalidOperationException(
                        $"Unable to find card details for account {cardDetails.AccountId}");
                }
            }
            
            TakePayment(cardDetails);

            return base.Handle(@event);
        }

        private void TakePayment(AccountCardDetails cardDetails)
        {
            //TODO: We are just logging here, over calling a payment provider, which is what you would really want to do
            _logger.Log(LogLevel.Information,
                "Payment request for account with CC {0} and CVC {1}",
                cardDetails.CardNumber,
                cardDetails.CardSecurityCode);
        }
    }
}