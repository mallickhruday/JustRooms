using System;
using System.Threading;
using System.Threading.Tasks;
using DirectBooking.adapters.data;
using DirectBooking.application;
using DirectBooking.ports.commands;
using DirectBooking.ports.events;
using DirectBooking.ports.repositories;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;

namespace DirectBooking.ports.handlers
{
    public class BookGuestRoomOnAccountHandlerAsync : RequestHandlerAsync<BookGuestRoomOnAccount>
    {
        private readonly DbContextOptions<BookingContext> _options;
        private readonly IAmACommandProcessor _messagePublisher;

        public BookGuestRoomOnAccountHandlerAsync(DbContextOptions<BookingContext> options, IAmACommandProcessor messagePublisher)
        {
            _options = options;
            _messagePublisher = messagePublisher;
        }
        public override async Task<BookGuestRoomOnAccount> HandleAsync(BookGuestRoomOnAccount command, CancellationToken cancellationToken = new CancellationToken())
        {
            Guid messageId;
            using (var uow = new BookingContext(_options))
            {
                using (var trans = uow.Database.BeginTransaction())
                {
                    var repository = new RoomBookingRepositoryAsync(new EFUnitOfWork(uow));

                    var roomBooking = new RoomBooking(
                        command.BookingId,
                        command.DateOfFirstNight,
                        command.NumberOfNights,
                        command.NumberOfGuests,
                        command.Type,
                        command.Price,
                        command.AccountId);

                    await repository.AddAsync(roomBooking, cancellationToken);

                    messageId = _messagePublisher.DepositPost(new GuestRoomBookingMade
                    {
                        BookingId = roomBooking.RoomBookingId.ToString(),
                        DateOfFirstNight = roomBooking.DateOfFirstNight,
                        NumberOfGuests = roomBooking.NumberOfGuests,
                        NumberOfNights = roomBooking.NumberOfNights,
                        Type = roomBooking.RoomType,
                        Price = roomBooking.Price,
                        AccountId = roomBooking.AccountId,
                    });
                    
                    trans.Commit();
                }
            }

            _messagePublisher.ClearOutbox(messageId);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}