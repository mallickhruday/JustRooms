using System;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Application;

namespace Accounts.Ports.Repositories
{
    /// <summary>
    /// An abstraction over a storage layer
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Delete the item, all versions
        /// </summary>
        /// <param name="accountId">The account to delete</param>
        /// <param name="version">The version to delete, defaults to the snapshot version, deleting visibility but preserving history</param>
        /// <param name="ct">Cancel the operation</param>
        Task DeleteAsync(Guid accountId, string version = Account.SnapShot, CancellationToken ct = default(CancellationToken));
         
        /// <summary>
        /// Get the item, we default to the snapshot version, unless you return a current version of state
        /// </summary>
        /// <param name="accountId">The id of the account to load</param>
        /// <param name="version">The version number to load</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>THe matching account</returns>
        Task<Account> GetAsync(Guid accountId, string version = Account.SnapShot, CancellationToken ct = default(CancellationToken));
        
        /// <summary> 
        /// Save the item
        /// </summary>
        /// <param name="account">The account to save</param>
        /// <param name="ct">Cancel the operataion</param>
        Task SaveAsync(Account account, CancellationToken ct = default(CancellationToken));
        
   }
}