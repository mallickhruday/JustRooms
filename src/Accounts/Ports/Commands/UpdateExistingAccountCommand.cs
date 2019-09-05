using System;
using System.Collections.Generic;
using Accounts.Application;
using Paramore.Brighter;

namespace Accounts.Ports.Commands
{
    /// <summary>
    /// Update an existing account
    /// </summary>
    public class UpdateExistingAccountCommand : Command
    {
        /// <summary>
        ///  Construvt an update account command
        /// </summary>
        /// <param name="accountId">The id of the guest account to update</param>
        /// <param name="name">The name of the guest</param>
        /// <param name="addresses">The addresses of the guest, should at least include Billing</param>
        /// <param name="contactDetails">The guest's contact details</param>
        /// <param name="cardDetails">The guest's 16 digit card number and CVC code</param>
        public UpdateExistingAccountCommand(Guid accountId, Name name, List<Address> addresses, ContactDetails contactDetails, CardDetails cardDetails) 
            : base(Guid.NewGuid())
        {
            AccountId = accountId;
            Addresses = addresses;
            ContactDetails = contactDetails;
            CardDetails = cardDetails;
        }
        
        /// <summary>
        /// The guest's account id
        /// </summary>
        public Guid AccountId { get; set; }
        
        /// <summary>
        /// The guest's name
        /// </summary>
        public Name Name { get; set; }
        
        /// <summary>
        /// The guest's addresses
        /// </summary>
        public List<Address> Addresses { get; set; }
        
        /// <summary>
        /// The guest's contact details
        /// </summary>
        public ContactDetails ContactDetails { get; set; }
        
        /// <summary>
        /// The guest's card details
        /// </summary>
        public CardDetails CardDetails { get; set; }
        
        /// <summary>
        /// Who is issuing this command
        /// </summary>
        public string LockBy { get; set; }
    }
}