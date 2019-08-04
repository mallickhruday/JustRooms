namespace Accounts.Application
{
    public class Address
    {
        public Address(string fistLineOfAddress, AddressType addressType, string state, string zipCode)
        {
            FistLineOfAddress = fistLineOfAddress;
            AddressType = addressType;
            State = state;
            ZipCode = zipCode;
        }

        public AddressType AddressType { get; }
        public string FistLineOfAddress { get; }
        public string ZipCode { get; }
        public string State { get; }
        
    }
}