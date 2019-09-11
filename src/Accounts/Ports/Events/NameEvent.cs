namespace Accounts.Ports.Events
{
    /// <summary>
    /// The guest's name
    /// </summary>
    public class NameEvent
    { 
        /// <summary>
        /// The guest's first name
        /// </summary>
        public string FirstName { get; set; }
        
        /// <summary>
        /// The guest's last nam,e
        /// </summary>
        public string LastName { get; set; }
    }
}