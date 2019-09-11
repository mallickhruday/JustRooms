using System;
using DirectBooking.application;
using Paramore.Brighter;

namespace DirectBooking.ports.events
{
    public class GuestRoomBookingMade : Event
    {
        public GuestRoomBookingMade() : base(Guid.NewGuid()) {}
        
        public string BookingId { get; set; }
        public DateTime DateOfFirstNight { get; set; }
        public RoomType Type { get; set; }
        public double Price { get; set; }
        public int NumberOfNights { get; set; }
        public int NumberOfGuests { get; set; }
        public string AccountId { get; set; }
    }
}