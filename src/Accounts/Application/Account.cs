using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;
using Accounts.Application.Converters;

namespace Accounts.Application
{
    [DynamoDBTable("Accounts")]
    public class Account
    {
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
        
        [DynamoDBVersion]
        public int? VersionNumber { get; set; }

        public Account() {}

        public Account(Guid accountId, Name name,  List<Address> addresses, ContactDetails contactDetails, CardDetails cardDetails)
        {
            AccountId = accountId.ToString();
            Name = name;
            Addresses = addresses;
            ContactDetails = contactDetails;
            CardDetails = cardDetails;
        }
 
    }
}