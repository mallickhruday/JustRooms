using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;
using Accounts.Application.Converters;

namespace Accounts.Application
{
    [DynamoDBTable("Accounts")]
    public class Account
    {
        public const string SnapShot = "V0";
        public const string VersionPrefix = "V";
        
        [DynamoDBHashKey]
        [DynamoDBProperty]
        public string AccountId { get; set; }
        
        [DynamoDBProperty(typeof(NameTypeConverter ))]
        public Name Name { get; set; }
        
        [DynamoDBProperty(typeof(AddressListTypeConverter))]
        public List<Address> Addresses { get; set; }
        
        [DynamoDBProperty(typeof(ContactDetailsTypeConverter))]
        public ContactDetails ContactDetails { get; set; }
        
        [DynamoDBProperty(typeof(CardDetailsTypeConverter))]
        public CardDetails CardDetails { get; set; }
        
        [DynamoDBRangeKey]
        [DynamoDBProperty]
        public string Version { get; set; }
        
        [DynamoDBProperty]
        public int CurrentVersion { get; set; }
        
        [DynamoDBProperty]
        public string LockedBy { get; set; }

        [DynamoDBProperty]
        public string LockExpiresAt { get; set; }

        public Account() {}

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