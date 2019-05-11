using JustSaying.Models;

namespace JustRooms.GuestDirectBooking.ports.events
{
    internal class GuestRoomBookingMade : Message
    {
        public RoomType Type { get; set; }
        public Money Price { get; set; }
        public int BookingId { get; set; }
        public int NumberOfNights { get; set; }
        public int NumberOfGuests { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FistLineOfAddress { get; set; }
        public string ZipCode { get; set; }
        public string State { get; set; }
        public string TelephoneNumber {get;set;}
        public string CardNumber { get; set; }
        public string CardSecurityCode { get; set; }
    }
}