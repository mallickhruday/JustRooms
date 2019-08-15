using System;
using Accounts.Ports.Results;
using Paramore.Darker;

namespace Accounts.Ports.Queries
{
    public class GetAccountById : IQuery<AccountResult>
    {
        public Guid AccountId { get; }

        public GetAccountById(Guid accountId)
        {
            AccountId = accountId;
        }
    }
    
}