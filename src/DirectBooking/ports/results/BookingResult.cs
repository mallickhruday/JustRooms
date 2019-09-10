using System;
using DirectBooking.application;

namespace JustRoomsTests.DirectBooking.ports.results
{
    public class BookingResult
    {
        /// <summary>
        /// The id of this booking
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
        /// The type of room the booking is for
        /// </summary>
        public RoomType RoomType { get; set; }

        /// <summary>
        /// The price per night of the booking
        /// </summary>
        public Money Price { get; set; }
        
        /// <summary>
        /// The account that the booking is for, if any
        /// </summary>
        public string AccountId { get; set; }
        
        public BookingResult(RoomBooking roomBooking)
        {
            BookingId = roomBooking.RoomBookingId.ToString();
            DateOfFirstNight = roomBooking.DateOfFirstNight;
            NumberOfGuests = roomBooking.NumberOfGuests;
            NumberOfNights = roomBooking.NumberOfNights;
            RoomType = roomBooking.RoomType;
            Price = roomBooking.Price;
            AccountId = roomBooking.AccountId;
        }

      }
}