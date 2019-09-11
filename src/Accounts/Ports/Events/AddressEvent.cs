namespace Accounts.Ports.Events
{
    /// <summary>
    /// A guest's addresws
    /// </summary>
    public class AddressEvent
    {
        /// <summary>
        /// The type of address: Billing, Work, Home
        /// </summary>
        public string AddressType { get; set; }
        
        /// <summary>
        /// The first line of the address
        /// </summary>
        public string FistLineOfAddress { get; set; }
        
        /// <summary>
        /// The customer's zipcode
        /// </summary>
        public string ZipCode { get; set; }
        
        /// <summary>
        /// The US state
        /// </summary>
        public string State { get; set; }
        
    }
}