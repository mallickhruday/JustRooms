using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Amazon.DynamoDBv2.DataModel;
using Accounts.Application.Converters;

namespace Accounts.Application
{
    /// <summary>
    /// A Guest Account
    /// </summary>
    public class Account
    {
        /// <summary>
        /// The id of the guest account
        /// </summary>
        public Guid AccountId { get; set; }
        
        /// <summary>
        /// The guest's name
        /// </summary>
        public Name Name { get; set; }
        
        /// <summary>
        /// The guest's addresses, should at least include billing
        /// </summary>
        public List<Address> Addresses { get; set; }
        
        /// <summary>
        /// The guests contact details
        /// </summary>
        public ContactDetails ContactDetails { get; set; }
        
        /// <summary>
        /// The guest's credit card details
        /// </summary>
        public CardDetails CardDetails { get; set; }
        
        /// <summary>
        /// Which is the current version of the guest record (accurate only on the 'V0' snapshot)
        /// </summary>
        [Timestamp]
        public byte[] Version { get; set; }
        
        /// <summary>
        /// Who has locked this record for editing, if anyone?
        /// </summary>
        public string LockedBy { get; set; }

        /// <summary>
        /// When does the lock expire
        /// </summary>
        public string LockExpiresAt { get; set; }

        /// <summary>
        /// Construct an empty account, primarily intended for deserialization not user use
        /// </summary>
        public Account() {}

        /// <summary>
        /// Create a guest account record
        /// </summary>
        /// <param name="accountId">The id of the guest account</param>
        /// <param name="name">The name of the guest account</param>
        /// <param name="addresses">The address of the guest account</param>
        /// <param name="contactDetails">The contact details of the guest</param>
        /// <param name="cardDetails">The card details of the guest</param>
        public Account(Guid accountId, Name name,  List<Address> addresses, ContactDetails contactDetails, CardDetails cardDetails)
        {
            AccountId = accountId;
            Name = name;
            Addresses = addresses;
            ContactDetails = contactDetails;
            CardDetails = cardDetails;
        }
    }
}