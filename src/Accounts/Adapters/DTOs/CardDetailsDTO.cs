namespace Accounts.Adapters.DTOs
{
    /// <summary>
    /// The guest's credit card details
    /// </summary>
    public class CardDetailsDTO
    {
        /// <summary>
        /// The 18 digit card number
        /// </summary>
        public string CardNumber { get; set; }
        
        /// <summary>
        /// The CVC code of the card
        /// </summary>
        public string CardSecurityCode { get; set; }
    }
}