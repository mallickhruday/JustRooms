using Amazon.DynamoDBv2.DataModel;

namespace CreditCardsAccountStreamReader.Application
{
    [DynamoDBTable("CardDetails")]
    public class AccountCardDetails
    {
        public const string SnapShot = "V0";
        public const string VersionPrefix = "V";
        
        [DynamoDBHashKey]
        [DynamoDBProperty]
        public string AccountId { get; set; }
        [DynamoDBProperty]
        public string CardNumber { get; set; }
        [DynamoDBProperty]
        public string CardSecurityCode { get; set; }
        [DynamoDBProperty]
        public string FirstLineOfAddress { get; set; }
        [DynamoDBProperty]
        public string Name { get; set; }
        [DynamoDBProperty]
        public string ZipCode { get; set; }
        [DynamoDBProperty]
        public int CurrentVersion { get; set; }
        [DynamoDBProperty]
        [DynamoDBRangeKey]
        public string Version { get; set; }
        [DynamoDBProperty]
        public string LockedBy { get; set; }
        [DynamoDBProperty]
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