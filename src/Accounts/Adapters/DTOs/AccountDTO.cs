using System.Collections.Generic;
using System.Linq;
using Accounts.Ports.Results;

namespace Accounts.Adapters.DTOs
{
    public class AccountDTO
    {
        public string AccountId { get; set; }
        public NameDTO Name { get; set; }
        public List<AddressDTO> Addresses { get; set; }
        public ContactDetailsDTO ContactDetails { get; set; }
        public CardDetailsDTO CardDetails { get; set; }

        public static AccountDTO FromQueryResult(AccountResult accountResult)
        {
            return new AccountDTO
            {
                AccountId = accountResult.Id.ToString(),
                Name = new NameDTO {FirstName = accountResult.Name.FirstName, LastName = accountResult.Name.LastName},
                Addresses = accountResult.Addresses.Select(addr =>
                    new AddressDTO
                    {
                        FistLineOfAddress = addr.FistLineOfAddress,
                        AddressType = addr.AddressType.ToString(),
                        State = addr.State,
                        ZipCode = addr.ZipCode
                    }).ToList(),
                CardDetails = new CardDetailsDTO
                {
                    CardNumber = accountResult.CardDetails.CardNumber,
                    CardSecurityCode = accountResult.CardDetails.CardNumber
                },
                ContactDetails = new ContactDetailsDTO
                {
                    Email = accountResult.ContactDetails.Email,
                    TelephoneNumber = accountResult.ContactDetails.TelephoneNumber
                }
            };
        }
     }
}