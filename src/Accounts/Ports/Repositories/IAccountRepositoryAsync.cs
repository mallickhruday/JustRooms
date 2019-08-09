using System;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Application;

namespace Accounts.Ports.Repositories
{
    public interface IAccountRepositoryAsync
    {
        /// <summary>
        /// Adds a new current record to the database and a version 1 record
        /// </summary>
        /// <param name="account">The account to add</param>
        /// <param name="ct">Operation cancellation</param>
        /// <returns></returns>
        Task AddAsync(Account account, CancellationToken ct = default(CancellationToken));
        
        /// <summary>
        /// Delete the account by removing the 'current record' but leave history
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task DeleteAsync(Guid accountId, CancellationToken ct = default(CancellationToken));
        
        /// <summary>
        /// Get the current record
        /// </summary>
        /// <param name="accountId">The account to get</param>
        /// <param name="ct">Operation cancellation</param>
        /// <returns></returns>
        Task<Account> GetAsync(Guid accountId, CancellationToken ct = default(CancellationToken));
        
        /// <summary>
        /// Lock the current record for 500ms whilst we edit it
        /// </summary>
        /// <param name="accountId">The account to lock</param>
        /// <param name="ct">The cancellation operation</param>
        /// <returns>Someone else holds a lock</returns>
        Task<bool> LockAsync(Guid accountId, CancellationToken ct = default(CancellationToken));
        Task UpdateAsync(Account account, CancellationToken ct = default(CancellationToken));
    }
}