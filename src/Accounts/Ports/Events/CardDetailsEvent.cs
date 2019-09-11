namespace Accounts.Ports.Events
{
    /// <summary>
    /// The guest's card details
    /// </summary>
    public class CardDetailsEvent
    {
        /// <summary>
        /// The guest's card number
        /// </summary>
        public string CardNumber { get; set; }
        
        /// <summary>
        /// The guest's security code
        /// </summary>
        public string CardSecurityCode { get; set; }
    }
}