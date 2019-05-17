using System;
using JustSaying.Models;

namespace JustRooms.DirectBookingEventConsumer.ports.events
{
    public class GuestRoomBookingMade : Message
    {
        public Guid BookingId { get; set; }
        public DateTime DateOfFirstNight { get; set; }
        public Money Price { get; set; }
        public int NumberOfNights { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}