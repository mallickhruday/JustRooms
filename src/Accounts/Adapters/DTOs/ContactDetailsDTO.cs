namespace Accounts.Adapters.DTOs
{
    /// <summary>
    /// The contact details of the guest
    /// </summary>
    public class ContactDetailsDTO
    {
        /// <summary>
        /// The guest's email address
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// The guest's telephone number
        /// </summary>
        public string TelephoneNumber { get; set; }
    }
}