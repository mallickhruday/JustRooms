using System;
using DirectBooking.application;
using Paramore.Brighter;

namespace DirectBooking.ports.commands
{
    public class BookGuestRoom : Command
    {
        
        public Guid BookingId { get; set; }
        public DateTime DateOfFirstNight { get; set; }
        public RoomType Type { get; set; }
        public Money Price { get; set; }
        public int NumberOfNights { get; set; }
        public int NumberOfGuests { get; set; }
        public string AccountId { get; set; }

        public BookGuestRoom() : 
            base(Guid.NewGuid())
        {
        }
    }
}