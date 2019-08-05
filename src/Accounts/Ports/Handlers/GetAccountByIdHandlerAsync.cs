using System.Threading;
using System.Threading.Tasks;
using Accounts.Ports.Queries;
using Accounts.Ports.Repositories;
using Accounts.Ports.Results;
using Paramore.Darker;

namespace Accounts.Ports.Handlers
{
    public class GetAccountByIdHandlerAsync : QueryHandlerAsync<GetAccountById, AccountByIdResult>
    {
        private readonly IAccountRepositoryAsync _accountRepositoryAsync;

        public GetAccountByIdHandlerAsync(IAccountRepositoryAsync accountRepositoryAsync)
        {
            _accountRepositoryAsync = accountRepositoryAsync;
        }
        
        public override async Task<AccountByIdResult> ExecuteAsync(GetAccountById query, CancellationToken cancellationToken = new CancellationToken())
        {
            var account = await _accountRepositoryAsync.GetAsync(query.AccountId, cancellationToken);
            return new AccountByIdResult(account);
        }
    }
}