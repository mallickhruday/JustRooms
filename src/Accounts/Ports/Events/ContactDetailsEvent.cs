namespace Accounts.Ports.Events
{
    /// <summary>
    /// The contact details for a guest
    /// </summary>
    public class ContactDetailsEvent
    {
        /// <summary>
        /// The guest's email
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// The guest's telephone number
        /// </summary>
        public string TelephoneNumber { get; set; }
    }
}