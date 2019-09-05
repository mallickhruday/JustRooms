namespace Accounts.Application
{
    /// <summary>
    /// A guest's name
    /// </summary>
    public class Name
    {
        /// <summary>
        /// Default constructor, mostly used for deserialization
        /// </summary>
        public Name(){}
        
        /// <summary>
        /// The name of a guest
        /// </summary>
        /// <param name="firstName">The guest's first name</param>
        /// <param name="lastName">The guest's second name</param>
        public Name(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        /// <summary>
        /// The guest's first name
        /// </summary>
        public string FirstName { get; }
        
        /// <summary>
        /// The guest's last name
        /// </summary>
        public string LastName { get; }
    }
}