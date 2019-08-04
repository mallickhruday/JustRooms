using System;
using System.Collections.Generic;

namespace DirectBooking.application
{
    public class GuestRoomBookingOnAccount
    {
        public Guid BookingId { get; set; }
        public DateTime DateOfFirstNight { get; set; }
        public int NumberOfNights { get; set; }
        public int NumberOfGuests { get; set; }
        public RoomType Type { get; set; }
        public Money Price { get; set; }
        
        public string AccountId { get; set; }
   }
}