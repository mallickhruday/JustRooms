using JustSaying.Models;

namespace JustRooms.DirectBookingEventConsumer.ports.events
{
    public class GuestRoomBookingMade : Message
    {
        public Money Price { get; set; }
        public int NumberOfNights { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FistLineOfAddress { get; set; }
        public string ZipCode { get; set; }
        public string State { get; set; }
        public string CardNumber { get; set; }
        public string CardSecurityCode { get; set; }
    }
}