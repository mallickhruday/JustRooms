using System;
using System.Globalization;
using JustRoomsTests.DirectBooking.ports.results;

namespace DirectBooking.adapters.dtos
{
    public class RoomBookingDTO
    {
        /// <summary>
        /// The identifier of the booking
        /// </summary>
        public string BookingId { get; set; }
        
        /// <summary>
        /// The date of the first night of the stay
        /// </summary>
        public DateTime DateOfFirstNight { get; set; }
        
        /// <summary>
        /// How many nights is the booking fpr
        /// </summary>
        public int NumberOfNights { get; set; }
        
        /// <summary>
        /// The number of guests in the booking
        /// </summary>
        public int NumberOfGuests { get; set; }

        /// <summary>
        /// The type of Room
        /// </summary>
        public string RoomType { get; set; }

        /// <summary>
        /// The price per night of the booking
        /// </summary>
        public string Amount { get; set; }
        
        /// <summary>
        /// The account that the booking is for, if any
        /// </summary>
        public string AccountId { get; set; }
        
        /// <summary>
        /// The currency of the transaction
        /// </summary>
        public string Currency { get; set; }

        public static RoomBookingDTO FromQueryResult(BookingResult booking)
        {
            return new RoomBookingDTO
            {
                BookingId = booking.BookingId,
                DateOfFirstNight = booking.DateOfFirstNight,
                NumberOfNights = booking.NumberOfNights,
                NumberOfGuests = booking.NumberOfGuests,
                RoomType = booking.RoomType.ToString(),
                Amount = Convert.ToString(booking.Price.Amount, CultureInfo.InvariantCulture),
                Currency = booking.Price.Currency,
                AccountId = booking.AccountId
            };
        }

    }
}