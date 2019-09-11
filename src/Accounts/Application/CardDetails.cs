using System;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Application
{
    /// <summary>
    /// The guest's card details
    /// </summary>
    public class CardDetails
    {
        public CardDetails()
        {
        }

        /// <summary>
        /// Construct new card detail;s
        /// </summary>
        /// <param name="account">The account that we belong to</param>
        /// <param name="cardNumber">The 16-digit card number</param>
        /// <param name="cardSecurityCode">The card's CVC code</param>
        public CardDetails(Account account, string cardNumber, string cardSecurityCode)
        {
            Account = account;
            AccountId = account.AccountId;
            CardNumber = cardNumber;
            CardSecurityCode = cardSecurityCode;
        }

        /// <summary>
        /// The id of these details
        /// </summary>
        public Guid CardDetailsId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The account that we belong to
        /// </summary
        public Guid AccountId { get; set; }

        /// <summary>
        /// The parent account
        /// </summary>
        public Account Account { get; set; }

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