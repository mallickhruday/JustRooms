using System.Threading;
using System.Threading.Tasks;
using Accounts.Adapters.Data;
using Accounts.Ports.Queries;
using Accounts.Ports.Repositories;
using Accounts.Ports.Results;
using Microsoft.EntityFrameworkCore;
using Paramore.Darker;

namespace Accounts.Ports.Handlers
{
    /// <summary>
    /// Retrieve an account by id
    /// </summary>
    public class GetAccountByIdHandlerAsync : QueryHandlerAsync<GetAccountById, AccountResult>
    {
        private readonly DbContextOptions<AccountContext> _options;

        /// <summary>
        /// Construct a handler for queries to retrieve guest accounts by id
        /// </summary>
        /// <param name="options"></param>
        public GetAccountByIdHandlerAsync(DbContextOptions<AccountContext> options)
        {
            _options = options;
        }
        
        /// <summary>
        /// Handle a request to retrieve an account by id
        /// </summary>
        /// <param name="query">A query describing getting an account by Id</param>
        /// <param name="cancellationToken">A token that allows us to cancel an ongoing operation</param>
        /// <returns>An AccountResult in response to that query</returns>
        public override async Task<AccountResult> ExecuteAsync(GetAccountById query, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = new AccountContext(_options))
            {
                using (var trans = uow.Database.BeginTransaction())
                {
                    var accountRepositoryAsync = new AccountRepositoryAsync(new EFUnitOfWork(uow));
                    var account = await accountRepositoryAsync.GetAsync(query.AccountId, cancellationToken);
                    return new AccountResult(account);
                }
            }
        }
    }
}