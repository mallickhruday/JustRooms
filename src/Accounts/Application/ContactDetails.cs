using System;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Application
{
    /// <summary>
    /// The contact details of the guest
    /// </summary>
    public class ContactDetails
    {
        /// <summary>
        /// Construct an empty set of contact details, mainly required for deserialization
        /// </summary>
        public ContactDetails()
        {
        }

        /// <summary>
        /// Construct contact details
        /// </summary>
        /// <param name="account">The account we belong to</param>
        /// <param name="email">The guest's email address</param>
        /// <param name="telephoneNumber">The guest's telephone number</param>
        public ContactDetails(Account account, string email, string telephoneNumber)
        {
            Account = account;
            AccountId = account.AccountId;
            Email = email;
            TelephoneNumber = telephoneNumber;
        }
        /// <summary>
        /// The id of the contact details
        /// </summary>
        public Guid ContactDetailsId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The account that we belong to
        /// </summary>
        public Guid AccountId { get; set; }

        /// <summary>
        /// The parent account
        /// </summary>
        public Account Account { get; set; }

        /// <summary>
        /// The guest's email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The geust's telephone number
        /// </summary>
        public string TelephoneNumber { get; set; }
    }
}