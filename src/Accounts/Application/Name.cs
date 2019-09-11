using System;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Application
{
    /// <summary>
    /// A guest's name
    /// </summary>
    public class Name
    {
        /// <summary>
        /// Default constructor, mostly used for deserialization
        /// </summary>
        public Name(){}
        
        /// <summary>
        /// The name of a guest
        /// </summary>
        /// <param name="firstName">The guest's first name</param>
        /// <param name="lastName">The guest's second name</param>
        public Name(Account account, string firstName, string lastName)
        {
            Account = account;
            AccountId = account.AccountId;
            FirstName = firstName;
            LastName = lastName;
        }
        
        /// <summary>
        /// The id of our name
        /// </summary>
        public Guid NameId { get; set; } = Guid.NewGuid();
        
        /// <summary>
        /// The account that we belong to
        /// </summary>
        public Guid AccountId { get; set; }

        /// <summary>
        /// The parent account
        /// </summary>
        public Account Account { get; set; }
         
        /// <summary>
        /// The guest's first name
        /// </summary>
        public string FirstName { get; set; }
        
        /// <summary>
        /// The guest's last name
        /// </summary>
        public string LastName { get; set; }
    }
}