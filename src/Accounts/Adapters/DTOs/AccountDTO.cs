using System.Collections.Generic;
using System.Linq;
using Accounts.Ports.Results;

namespace Accounts.Adapters.DTOs
{
    /// <summary>
    /// A guest account
    /// </summary>
    public class AccountDTO
    {
        /// <summary>
        /// The id of the account
        /// </summary>
        public string AccountId { get; set; }
        
        /// <summary>
        /// The guest's name
        /// </summary>
        public NameDTO Name { get; set; }
        
        /// <summary>
        /// The guests addresses, at least home and billing
        /// </summary>
        public List<AddressDTO> Addresses { get; set; }
        
        /// <summary>
        /// Contact details for the guest
        /// </summary>
        public ContactDetailsDTO ContactDetails { get; set; }
        
        /// <summary>
        /// The guest's credit card details
        /// </summary>
        public CardDetailsDTO CardDetails { get; set; }

        /// <summary>
        /// Turn a query result into a guest account data transfer obkect
        /// </summary>
        /// <param name="accountResult">The result of a guest account query</param>
        /// <returns></returns>
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