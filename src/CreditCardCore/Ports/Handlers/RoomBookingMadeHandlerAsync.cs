using System;
using CreditCardCore.Ports.Events;
using CreditCardCore.Ports.Repositories;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace CreditCardCore.Ports.Handlers
{
    public class RoomBookingMadeHandlerAsync : RequestHandler<GuestRoomBookingMade>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RoomBookingMadeHandlerAsync> _logger;

        public RoomBookingMadeHandlerAsync(IUnitOfWork unitOfWork, ILogger<RoomBookingMadeHandlerAsync> _logger)
        {
            _unitOfWork = unitOfWork;
            this._logger = _logger;
        }
        public override GuestRoomBookingMade Handle(GuestRoomBookingMade @event)
        {
            var repo = new AccountCardDetailsRepositoryAsync(_unitOfWork);

            var cardDetails = repo.GetAsync(Guid.Parse(@event.AccountId)).GetAwaiter().GetResult();
            
            //TODO: We are just logging here, over calling a payment provider, which is what you would really want to do
            _logger.Log(LogLevel.Information, 
                "Payment request for account with CC {0} and CVC {1}", 
                cardDetails.CardNumber, 
                cardDetails.CardSecurityCode);
            
            return base.Handle(@event);
        }
   }
}