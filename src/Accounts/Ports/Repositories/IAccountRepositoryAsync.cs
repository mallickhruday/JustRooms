using System;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Application;

namespace Accounts.Ports.Repositories
{
    public interface IAccountRepositoryAsync
    {
        Task AddAsync(Account account, CancellationToken ct);
        Task<Account> GetAsync(Guid accountIdi, CancellationToken ct);
    }
}