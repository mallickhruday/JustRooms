using System.Threading;
using System.Threading.Tasks;
using DirectBooking.adapters.data;
using DirectBooking.ports.queries;
using DirectBooking.ports.repositories;
using JustRoomsTests.DirectBooking.ports.results;
using Microsoft.EntityFrameworkCore;
using Paramore.Darker;

namespace DirectBooking.ports.handlers
{
    public class GetBookingByIdHandlerAsync : QueryHandlerAsync<GetBookingById, BookingResult>
    {
        private readonly DbContextOptions<BookingContext> _options;

        public GetBookingByIdHandlerAsync(DbContextOptions<BookingContext> options)
        {
            _options = options;
        }
        public override async Task<BookingResult> ExecuteAsync(GetBookingById query, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = new BookingContext(_options))
            {
                var repository = new RoomBookingRepositoryAsync(new EFUnitOfWork(uow));
                
                var booking = await repository.GetAsync(query.BookingId, cancellationToken);

                return new BookingResult(booking);
            }
        }
    }
}