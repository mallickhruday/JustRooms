using System.Threading;
using System.Threading.Tasks;
using Accounts.Ports.Queries;
using Accounts.Ports.Repositories;
using Accounts.Ports.Results;
using Paramore.Darker;

namespace Accounts.Ports.Handlers
{
    /// <summary>
    /// Retrieve an account by id
    /// </summary>
    public class GetAccountByIdHandlerAsync : QueryHandlerAsync<GetAccountById, AccountResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Construct a handler for queries to retrieve guest accounts by id
        /// </summary>
        /// <param name="unitOfWork"></param>
        public GetAccountByIdHandlerAsync(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        /// <summary>
        /// Handle a request to retrieve an account by id
        /// </summary>
        /// <param name="query">A query describing getting an account by Id</param>
        /// <param name="cancellationToken">A token that allows us to cancel an ongoing operation</param>
        /// <returns>An AccountResult in response to that query</returns>
        public override async Task<AccountResult> ExecuteAsync(GetAccountById query, CancellationToken cancellationToken = new CancellationToken())
        {
            var accountRepositoryAsync = new AccountRepositoryAsync(_unitOfWork);
            var account = await accountRepositoryAsync.GetAsync(query.AccountId, cancellationToken);
            return new AccountResult(account);
        }
    }
}