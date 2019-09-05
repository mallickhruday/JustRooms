using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;
using Accounts.Application.Converters;

namespace Accounts.Application
{
    /// <summary>
    /// A Guest Account
    /// </summary>
    [DynamoDBTable("Accounts")]
    public class Account
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
        /// The id of the guest account
        /// </summary>
        [DynamoDBHashKey]
        [DynamoDBProperty]
        public string AccountId { get; set; }
        
        /// <summary>
        /// The guest's name
        /// </summary>
        [DynamoDBProperty(typeof(NameTypeConverter ))]
        public Name Name { get; set; }
        
        /// <summary>
        /// The guest's addresses, should at least include billing
        /// </summary>
        [DynamoDBProperty(typeof(AddressListTypeConverter))]
        public List<Address> Addresses { get; set; }
        
        /// <summary>
        /// The guests contact details
        /// </summary>
        [DynamoDBProperty(typeof(ContactDetailsTypeConverter))]
        public ContactDetails ContactDetails { get; set; }
        
        /// <summary>
        /// The guest's credit card details
        /// </summary>
        [DynamoDBProperty(typeof(CardDetailsTypeConverter))]
        public CardDetails CardDetails { get; set; }
        
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
        /// Construct an empty account, primarily intended for deserialization not user use
        /// </summary>
        public Account() {}

        /// <summary>
        /// Create a guest account record
        /// </summary>
        /// <param name="accountId">The id of the guest account</param>
        /// <param name="name">The name of the guest account</param>
        /// <param name="addresses">The address of the guest account</param>
        /// <param name="contactDetails">The contact details of the guest</param>
        /// <param name="cardDetails">The card details of the guest</param>
        public Account(Guid accountId, Name name,  List<Address> addresses, ContactDetails contactDetails, CardDetails cardDetails)
        {
            AccountId = accountId.ToString();
            Name = name;
            Addresses = addresses;
            ContactDetails = contactDetails;
            CardDetails = cardDetails;
            Version = SnapShot;
            CurrentVersion = 0;
        }
    }
}