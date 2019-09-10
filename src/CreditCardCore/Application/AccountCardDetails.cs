using System;
using System.ComponentModel.DataAnnotations;
using Amazon.DynamoDBv2.DataModel;

namespace CreditCardCore.Application
{
    public class AccountCardDetails
    {
        public Guid AccountId { get; set; }
        
        public string CardNumber { get; set; }
        
        public string CardSecurityCode { get; set; }
        
        public string FirstLineOfAddress { get; set; }
        
        public string Name { get; set; }
        
        public string ZipCode { get; set; }
        
        [Timestamp]
        public byte[] Version { get; set; }
        
        public string LockedBy { get; set; }
        
        public string LockExpiresAt { get; set; }


        public AccountCardDetails() {}

        public AccountCardDetails(Guid accountId, string name, string cardNumber, string cardSecurityCode,
            string firstLineOfAddress, string zipCode)
        {
            AccountId = accountId;
            CardNumber = cardNumber;
            CardSecurityCode = cardSecurityCode;
            FirstLineOfAddress = firstLineOfAddress;
            Name = name;
            ZipCode = zipCode;
        }
    }
}