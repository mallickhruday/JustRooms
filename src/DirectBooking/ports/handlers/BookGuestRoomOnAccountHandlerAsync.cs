using System.Threading;
using System.Threading.Tasks;
using DirectBooking.application;
using DirectBooking.ports.commands;
using DirectBooking.ports.events;
using DirectBooking.ports.repositories;
using JustSaying.Messaging;
using Paramore.Brighter;

namespace DirectBooking.ports.handlers
{
    public class BookGuestRoomOnAccountHandlerAsync : RequestHandlerAsync<BookGuestRoomOnAccount>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMessagePublisher _messagePublisher;

        public BookGuestRoomOnAccountHandlerAsync(IUnitOfWork unitOfWork, IMessagePublisher messagePublisher)
        {
            _unitOfWork = unitOfWork;
            _messagePublisher = messagePublisher;
        }
        public override async Task<BookGuestRoomOnAccount> HandleAsync(BookGuestRoomOnAccount command, CancellationToken cancellationToken = new CancellationToken())
        {
            var repository = new RoomBookingRepositoryAsync(_unitOfWork);

            var roomBooking = new RoomBooking(
                command.BookingId, 
                command.DateOfFirstNight, 
                command.NumberOfNights, 
                command.NumberOfGuests, 
                command.Type, 
                command.Price, 
                command.AccountId);
            
            await repository.AddAsync(roomBooking, cancellationToken);

            await _messagePublisher.PublishAsync(new GuestRoomBookingMade
            {
                BookingId = roomBooking.BookingId,
                DateOfFirstNight = roomBooking.DateOfFirstNight,
                NumberOfGuests = roomBooking.NumberOfGuests,
                NumberOfNights = roomBooking.NumberOfNights,
                Type = roomBooking.RoomType,
                Price = roomBooking.Price,
                AccountId = roomBooking.AccountId,
            }, null, cancellationToken);
            
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}