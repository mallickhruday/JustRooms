using System;
using DirectBooking.application;
using JustSaying.Models;

namespace DirectBooking.ports.events
{
    public class GuestRoomBookingMade : Message
    {
        public string BookingId { get; set; }
        public DateTime DateOfFirstNight { get; set; }
        public RoomType Type { get; set; }
        public Money Price { get; set; }
        public int NumberOfNights { get; set; }
        public int NumberOfGuests { get; set; }
        public string AccountId { get; set; }
    }
}