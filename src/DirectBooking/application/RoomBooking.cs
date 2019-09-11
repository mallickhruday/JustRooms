using System;
using System.ComponentModel.DataAnnotations;

namespace DirectBooking.application
{
    /// <summary>
    /// A guest room booking
    /// TODO: For Bookings that are not on account we would need more details about the booking i.e. first name etc
    /// </summary>
    public class RoomBooking
    {
        /// <summary>
        /// The id of a booking
        /// </summary>
        public Guid RoomBookingId { get; set; }
        
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
        public double Price { get; set; }
        
        /// <summary>
        /// The account that the booking is for, if any
        /// </summary>
        public string AccountId { get; set; }
        
        /// <summary>
        /// The version of this record
        /// </summary>
        [Timestamp]
        public byte[] Version { get; set; }
        
        /// <summary>
        /// Who has locked this record for editing, if anyone?
        /// </summary>
        public string LockedBy { get; set; }

        /// <summary>
        /// When does the lock expire
        /// </summary>
        public string LockExpiresAt { get; set; }
        
        
        /// <summary>
        /// Default constructor. Mainly used for serialization.
        /// </summary>
        public RoomBooking(){}
        
        public RoomBooking(
            Guid roomBookingId,
            DateTime dateOfFirstNight,
            int numberOfNights,
            int numberOfGuests,
            RoomType roomType,
            double price,
            string accountId
            )
        {
            RoomBookingId = roomBookingId;
            DateOfFirstNight = dateOfFirstNight;
            NumberOfNights = numberOfNights;
            NumberOfGuests = numberOfGuests;
            RoomType = roomType;
            Price = price;
            AccountId = accountId;
        }

         

   }
}