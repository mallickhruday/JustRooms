namespace Accounts.Application
{
    public class ContactDetails
    {
        public ContactDetails(){}
        
        public ContactDetails(string email, string telephoneNumber)
        {
            Email = email;
            TelephoneNumber = telephoneNumber;
        }

        public string Email { get; }
        public string TelephoneNumber {get;}
    }
}