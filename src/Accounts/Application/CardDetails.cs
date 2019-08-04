namespace Accounts.Application
{
    public class CardDetails
    {
        public CardDetails() {}
        
        public CardDetails(string cardNumber, string cardSecurityCode)
        {
            CardNumber = cardNumber;
            CardSecurityCode = cardSecurityCode;
        }

        public string CardNumber { get; }
        public string CardSecurityCode { get; }
    }
}