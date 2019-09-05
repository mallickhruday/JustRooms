using System;
using Accounts.Ports.Results;
using Paramore.Darker;

namespace Accounts.Ports.Queries
{
    /// <summary>
    ///  A Query to retrieve a guest account by Id
    /// </summary>
    public class GetAccountById : IQuery<AccountResult>
    {
        /// <summary>
        /// The Id of the account to retrieve
        /// </summary>
        public Guid AccountId { get; }
        
        /// <summary>
        /// Constructor for an account by id query
        /// </summary>
        /// <param name="accountId">The account to search for</param>
        public GetAccountById(Guid accountId)
        {
            AccountId = accountId;
        }
    }
    
}