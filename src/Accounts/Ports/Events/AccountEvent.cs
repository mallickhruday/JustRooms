using System;
using System.Collections.Generic;
using Paramore.Brighter;

namespace Accounts.Ports.Events
{
    /// <summary>
    /// Something happened in the accounts system
    /// </summary>
    public class AccountEvent : Event
    {
        /// <summary>
        /// The identifier of the account
        /// </summary>
        public string AccountId { get; set; }
        
        /// <summary>
        /// The name of the guest
        /// </summary>
        public NameEvent Name { get; set; }
        
        /// <summary>
        /// The guests addresses
        /// </summary>
        public List<AddressEvent> Addresses { get; set; }
        
        /// <summary>
        /// The contact details for the guest
        /// </summary>
        public ContactDetailsEvent ContactDetails { get; set; }
        
        /// <summary>
        /// The card details for the guest
        /// </summary>
        public CardDetailsEvent CardDetails { get; set; }
        
        /// <summary>
        /// The version of the record
        /// </summary>
        public byte[] Version { get; set; }
        
        /// <summary>
        /// The account event
        /// </summary>

        public AccountEvent() : base(Guid.NewGuid()) {}
    }
}