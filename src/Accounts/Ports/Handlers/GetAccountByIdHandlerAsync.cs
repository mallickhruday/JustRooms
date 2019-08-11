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
        private readonly IUnitOfWork _unitOfWork;

        public GetAccountByIdHandlerAsync(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public override async Task<AccountByIdResult> ExecuteAsync(GetAccountById query, CancellationToken cancellationToken = new CancellationToken())
        {
            var accountRepositoryAsync = new AccountRepositoryAsync(_unitOfWork);
            var account = await accountRepositoryAsync.GetAsync(query.AccountId, cancellationToken);
            return new AccountByIdResult(account);
        }
    }
}