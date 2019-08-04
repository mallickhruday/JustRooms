using System;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Application;

namespace Accounts.Ports.Repositories
{
    public interface IAccountRepositoryAsync
    {
        Task AddAsync(Account account, CancellationToken ct = default(CancellationToken));
        Task<Account> GetAsync(Guid accountId, CancellationToken ct = default(CancellationToken));
        Task UpdateAsync(Account account, CancellationToken ct = default(CancellationToken));
        Task DeleteAsync(Guid accountAccountId, CancellationToken ct = default(CancellationToken));
    }
}