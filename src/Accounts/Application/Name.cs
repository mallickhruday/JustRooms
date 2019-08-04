namespace Accounts.Application
{
    public class Name
    {
        public Name(){}
        
        public Name(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string FirstName { get; }
        public string LastName { get; }
    }
}