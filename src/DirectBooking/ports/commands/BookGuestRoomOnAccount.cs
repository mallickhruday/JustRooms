using System;
using DirectBooking.application;
using Paramore.Brighter;

namespace DirectBooking.ports.commands
{
    /// <summary>
    /// Books a guest room on account
    /// </summary>
    public class BookGuestRoomOnAccount : Command
    {
        /// <summary>
        /// The id for  this booking
        /// </summary>
        public Guid BookingId { get; set; }
        
        /// <summary>
        /// The first night's  stay
        /// </summary>
        public DateTime DateOfFirstNight { get; set; }
        
        /// <summary>
        /// The type of room
        /// </summary>
        public RoomType Type { get; set; }
        
        /// <summary>
        /// The cost of the room, per night
        /// </summary>
        public Money Price { get; set; }
        
        /// <summary>
        /// The number of nights of the booking
        /// </summary>
        public int NumberOfNights { get; set; }
        
        /// <summary>
        /// How many guests in the room
        /// </summary>
        public int NumberOfGuests { get; set; }
        
        /// <summary>
        /// The account the booking is for
        /// </summary>
        public string AccountId { get; set; }

        public BookGuestRoomOnAccount() : 
            base(Guid.NewGuid())
        {
        }
    }
}