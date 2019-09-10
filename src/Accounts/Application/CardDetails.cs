using Microsoft.EntityFrameworkCore;

namespace Accounts.Application
{
    /// <summary>
    /// The guest's card details
    /// </summary>
    [Owned]
    public class CardDetails
    {
        public CardDetails() {}
        
        /// <summary>
        /// Construct new card detail;s
        /// </summary>
        /// <param name="cardNumber">The 16-digit card number</param>
        /// <param name="cardSecurityCode">The card's CVC code</param>
        public CardDetails(string cardNumber, string cardSecurityCode)
        {
            CardNumber = cardNumber;
            CardSecurityCode = cardSecurityCode;
        }

        /// <summary>
        /// The 16-digit card number
        /// </summary>
        public string CardNumber { get; set; }
        
        /// <summary>
        /// The CVC code of the card
        /// </summary>
        public string CardSecurityCode { get; set; }
    }
}