using System;
using System.Collections.Generic;
using Accounts.Application;

namespace Accounts.Ports.Results
{
     /// <summary>
     /// The result of an account query
     /// TODO: Technically we should not pass back the application types here, but a DTO
     /// We might weant to consider Automapper, though it is could be overkill
     /// </summary>
    public class AccountResult
    {
        /// <summary>
        /// Construct a query result
        /// </summary>
        /// <param name="account">The account to construct the result from</param>
        public AccountResult(Account account)
        {
            Id = account.AccountId;
            Name = account.Name;
            Addresses = account.Addresses;
            ContactDetails = account.ContactDetails;
            CardDetails = account.CardDetails;
        }

        /// <summary>
        /// The id of the guest account
        /// </summary>
        public Guid Id { get;}
        
        /// <summary>
        /// The name of the guest
        /// </summary>
        public Name Name { get;}
        
        /// <summary>
        /// The address of the guest, should include billing at least
        /// </summary>
        public List<Address> Addresses { get;}
        
        /// <summary>
        /// The contact details of the guest
        /// </summary>
        public ContactDetails ContactDetails { get;}
        
        /// <summary>
        /// The card details of the guest
        /// </summary>
        public CardDetails CardDetails { get;}
    }
}