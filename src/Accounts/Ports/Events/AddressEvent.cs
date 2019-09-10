namespace Accounts.Ports.Events
{
    public class AddressEvent
    {
        public string AddressType { get; set; }
        public string FistLineOfAddress { get; set; }
        public string ZipCode { get; set; }
        public string State { get; set; }
        
    }
}