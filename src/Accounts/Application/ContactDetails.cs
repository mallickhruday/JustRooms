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
        public ContactDetails(){}
        
        /// <summary>
        /// Construct contact details
        /// </summary>
        /// <param name="email">The guest's email address</param>
        /// <param name="telephoneNumber">The guest's telephone number</param>
        public ContactDetails(string email, string telephoneNumber)
        {
            Email = email;
            TelephoneNumber = telephoneNumber;
        }

        /// <summary>
        /// The guest's email
        /// </summary>
        public string Email { get; }
        
        /// <summary>
        /// The geust's telephone number
        /// </summary>
        public string TelephoneNumber {get;}
    }
}