namespace Accounts.Application
{
    /// <summary>
    /// An address
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Construct a new address
        /// </summary>
        /// <param name="fistLineOfAddress">The first line of the address</param>
        /// <param name="addressType">The type of the address: Home, Billing or Work</param>
        /// <param name="state">The state the address is in</param>
        /// <param name="zipCode">The Zipcode for the address</param>
        public Address(string fistLineOfAddress, AddressType addressType, string state, string zipCode)
        {
            FistLineOfAddress = fistLineOfAddress;
            AddressType = addressType;
            State = state;
            ZipCode = zipCode;
        }

        /// <summary>
        /// The type of the address: Home, Billing, or Work
        /// </summary>
        public AddressType AddressType { get; }
        
        /// <summary>
        /// The first line of the address
        /// </summary>
        public string FistLineOfAddress { get; }
        
        /// <summary>
        /// The Zipcode of the address
        /// </summary>
        public string ZipCode { get; }
        
        /// <summary>
        /// THe state that the address is in
        /// </summary>
        public string State { get; }
        
    }
}