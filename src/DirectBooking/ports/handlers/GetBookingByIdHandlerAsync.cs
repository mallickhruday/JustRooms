using System.Threading;
using System.Threading.Tasks;
using DirectBooking.application;
using DirectBooking.ports.queries;
using DirectBooking.ports.repositories;
using JustRoomsTests.DirectBooking.ports.results;
using Paramore.Darker;

namespace DirectBooking.ports.handlers
{
    public class GetBookingByIdHandlerAsync : QueryHandlerAsync<GetBookingById, BookingResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetBookingByIdHandlerAsync(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public override async Task<BookingResult> ExecuteAsync(GetBookingById query, CancellationToken cancellationToken = new CancellationToken())
        {
            var booking = await _unitOfWork.GetAsync(query.BookingId, RoomBooking.SnapShot, cancellationToken);
            
            return new BookingResult(booking);
        }
    }
}