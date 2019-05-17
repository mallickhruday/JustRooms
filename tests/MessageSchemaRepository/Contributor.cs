namespace MessageSchemaRepository
{
    public class Contributor
    {
        public Contributor(string name, string email)
        {
            Name = name;
            Email = email;
        }
        public string Email { get; }
        public string Name { get; }
    }
}