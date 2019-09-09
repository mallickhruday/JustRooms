using System;
using System.Threading;
using System.Threading.Tasks;
using CreditCardCore.Application;

namespace CreditCardCore.Ports.Repositories
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// Delete the item, all versions
        /// </summary>
        /// <param name="accountId">The account to delete</param>
        /// <param name="ct">Cancel the operation</param>
        Task DeleteAsync(Guid accountId, string version = AccountCardDetails.SnapShot, CancellationToken ct = default(CancellationToken));
         
        /// <summary>
        /// Get the item, we default to the snapshot version, unless you return a current version of state
        /// </summary>
        /// <param name="accountId">The id of the account to load</param>
        /// <param name="version">The version number to load</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>THe matching account</returns>
        Task<AccountCardDetails> GetAsync(Guid accountId, string version = AccountCardDetails.SnapShot, CancellationToken ct = default(CancellationToken));
        
         /// <summary> 
        /// Save the item
        /// </summary>
        /// <param name="account">The account to save</param>
        /// <param name="ct">Cancel the operataion</param>
        Task SaveAsync(AccountCardDetails account, CancellationToken ct = default(CancellationToken));
        
   }
}