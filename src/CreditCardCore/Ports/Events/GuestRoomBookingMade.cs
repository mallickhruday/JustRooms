using System;
using Paramore.Brighter;

namespace CreditCardCore.Ports.Events
{
    public class GuestRoomBookingMade : Event
    {
        public GuestRoomBookingMade() : base(Guid.NewGuid()) {}
        
        public string BookingId { get; set; }
        public Money Price { get; set; }
        public int NumberOfNights { get; set; }
        public int NumberOfGuests { get; set; }
        public string AccountId { get; set; }
    }
}