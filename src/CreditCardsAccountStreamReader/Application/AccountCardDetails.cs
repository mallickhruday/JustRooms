using Amazon.DynamoDBv2.DataModel;

namespace CreditCardsAccountStreamReader.Application
{
    [DynamoDBTable("CardDetails")]
    public class AccountCardDetails
    {
        public const string SnapShot = "V0";
        public const string VersionPrefix = "V";
        
        [DynamoDBHashKey]
        public string AccountId { get; }
        public string CardNumber { get; }
        public string CardSecurityCode { get; }
        public string FirstLineOfAddress { get; }
        public string Name { get; }
        public string ZipCode { get; }
        
        public int CurrentVersion { get; set; }
        public string Version { get; set; }
        public string LockedBy { get; set; }
        public string LockExpiresAt { get; set; }


        public AccountCardDetails() {}

        public AccountCardDetails(string accountId, string name, string cardNumber, string cardSecurityCode,
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