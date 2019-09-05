using System;
using System.Collections.Generic;
using Accounts.Application;
using Paramore.Brighter;

namespace Accounts.Ports.Commands
{
    /// <summary>
    /// Adds a new account
    /// </summary>
    public class AddNewAccountCommand : Command
    {
        /// <summary>
        /// Creates a new add account command
        /// </summary>
        /// <param name="name">The name of the guest</param>
        /// <param name="addresses">The addresses of the guest, should at least include billing</param>
        /// <param name="contactDetails">The guest's contact details</param>
        /// <param name="cardDetails">The guest's credit card details</param>
        public AddNewAccountCommand(Name name, List<Address> addresses, ContactDetails contactDetails, CardDetails cardDetails)
            : base(Guid.NewGuid())
        {
            Name = name;
            Addresses = addresses;
            ContactDetails = contactDetails;
            CardDetails = cardDetails;
        }
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public AddNewAccountCommand() : base(Guid.NewGuid()){}
        
        /// <summary>
        /// The name of the guest
        /// </summary>
        public Name Name { get; set; }
        
        /// <summary>
        /// The guest's addresses, at least one of which should be billing
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
    }
}