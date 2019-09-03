using System;
using System.Collections.Generic;
using Accounts.Application;

namespace Accounts.Ports.Results
{
    //TODO: Technically we should not pass back the application types here, but a DTO
    //We might weant to consider Automapper, though it is could be overkill
    public class AccountResult
    {
        public AccountResult(Account account)
        {
            Id = Guid.Parse(account.AccountId);
            Name = account.Name;
            Addresses = account.Addresses;
            ContactDetails = account.ContactDetails;
            CardDetails = account.CardDetails;
        }

        public Guid Id { get;}
        public Name Name { get;}
        public List<Address> Addresses { get;}
        public ContactDetails ContactDetails { get;}
        public CardDetails CardDetails { get;}
    }
}