namespace Accounts.Ports.Events
{
    public class CardDetailsEvent
    {
        public string CardNumber { get; set; }
        public string CardSecurityCode { get; set; }
    }
}