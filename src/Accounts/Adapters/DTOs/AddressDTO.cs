namespace Accounts.Adapters.DTOs
{
    /// <summary>
    /// The address of a guest
    /// </summary>
    public class AddressDTO
    {
        /// <summary>
        /// The type of address; Home, Billing or Work
        /// </summary>
        public string AddressType { get; set; }
        
        /// <summary>
        /// The first line of the address
        /// </summary>
        public string FistLineOfAddress { get; set; }
        
        /// <summary>
        /// The zipcoce
        /// </summary>
        public string ZipCode { get; set; }
        
        /// <summary>
        /// The US state
        /// </summary>
        public string State { get; set; }
        
    }
}