using System;

namespace Accounts.Application
{
    /// <summary>
    /// An address
    /// </summary>
    public class Address
    {
        public Address() {}
        
        /// <summary>
        /// Construct a new address
        /// </summary>
        /// <param name="fistLineOfAddress">The first line of the address</param>
        /// <param name="addressType">The type of the address: Home, Billing or Work</param>
        /// <param name="state">The state the address is in</param>
        /// <param name="zipCode">The Zipcode for the address</param>
        public Address(string fistLineOfAddress, AddressType addressType, string state, string zipCode)
        {
            AddressId = Guid.NewGuid();
            FistLineOfAddress = fistLineOfAddress;
            AddressType = addressType;
            State = state;
            ZipCode = zipCode;
        }
        
        public Guid AddressId { get; set; }

        /// <summary>
        /// The type of the address: Home, Billing, or Work
        /// </summary>
        public AddressType AddressType { get; set; }
        
        /// <summary>
        /// The first line of the address
        /// </summary>
        public string FistLineOfAddress { get; set; }
        
        /// <summary>
        /// The Zipcode of the address
        /// </summary>
        public string ZipCode { get; set; }
        
        /// <summary>
        /// THe state that the address is in
        /// </summary>
        public string State { get; set; }
        
    }
}