namespace Accounts.Ports.Policies
{
    /// <summary>
    /// Global identifiers for Polly policies to allow retrieval from the policy registry
    /// </summary>
    public class Catalog
    {
        /// <summary>
        /// A policy protecting access to a DynamoDb table
        /// </summary>
        public const string DynamoDbAccess = "DynamoDbAccess";
    }
}