using System;
using JustRoomsTests.DirectBooking.ports.results;
using Paramore.Darker;

namespace DirectBooking.ports.queries
{
    public class GetBookingById : IQuery<BookingResult>
    {
       public Guid BookingId { get; }

       public GetBookingById(Guid bookingId)
       {
           BookingId = bookingId;
       }
    }
}