using System;
using Amazon.DynamoDBv2.DataModel;
using DirectBooking.application.converters;

namespace DirectBooking.application
{
    /// <summary>
    /// A guest room booking
    /// TODO: For Bookings that are not on account we would need more details about the booking i.e. first name etc
    /// </summary>
    [DynamoDBTable("RoomBooking")]
    public class RoomBooking
    {
        /// <summary>
        /// The identifier of the snapshot record, which is the current state of an account
        /// </summary>
        public const string SnapShot = "V0";
        /// <summary>
        /// Used to precede the incrementing integer for the guest account version
        /// </summary>
        public const string VersionPrefix = "V";
         
        /// <summary>
        /// The id of a booking
        /// </summary>
        [DynamoDBHashKey]
        [DynamoDBProperty]
        public string BookingId { get; set; }
        
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
        [DynamoDBProperty(typeof(MoneyTypeConverter))]
        public Money Price { get; set; }
        
        /// <summary>
        /// The account that the booking is for, if any
        /// </summary>
        [DynamoDBProperty]
        public string AccountId { get; set; }
        
        /// <summary>
        /// The version of the guest account record
        /// </summary>
        [DynamoDBRangeKey]
        [DynamoDBProperty]
        public string Version { get; set; }
        
        /// <summary>
        /// Which is the current version of the guest record (accurate only on the 'V0' snapshot)
        /// </summary>
        [DynamoDBProperty]
        public int CurrentVersion { get; set; }
        
        /// <summary>
        /// Who has locked this record for editing, if anyone?
        /// </summary>
        [DynamoDBProperty]
        public string LockedBy { get; set; }

        /// <summary>
        /// When does the lock expire
        /// </summary>
        [DynamoDBProperty]
        public string LockExpiresAt { get; set; }
        
        
        /// <summary>
        /// Default constructor. Mainly used for serialization.
        /// </summary>
        public RoomBooking(){}
        
        public RoomBooking(
            string bookingId,
            DateTime dateOfFirstNight,
            int numberOfNights,
            int numberOfGuests,
            RoomType roomType,
            Money price,
            string accountId
            )
        {
            BookingId = bookingId;
            DateOfFirstNight = dateOfFirstNight;
            NumberOfNights = numberOfNights;
            NumberOfGuests = numberOfGuests;
            RoomType = roomType;
            Price = price;
            AccountId = accountId;
        }

         

   }
}