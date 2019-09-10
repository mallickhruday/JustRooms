using System;
using System.Threading;
using System.Threading.Tasks;
using CreditCardCore.Application;
using CreditCardCore.Ports.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CreditCardCore.Adapters.Data
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private CardDetailsContext _context;

        /// <summary>
        /// Construct a unit of work 
        /// </summary>
        /// <param name="context">The EF Context to use</param>
        public EFUnitOfWork(CardDetailsContext  context)
        {
            _context = context;
        }

        /// <summary>
        /// Delete the item, by default the snapshot version, which deletes the live record, but keeps history
        /// </summary>
        /// <param name="accountId">The account to delete</param>
        /// <param name="ct">Cancel the operation</param>
        public async Task DeleteAsync(Guid accountId, CancellationToken ct = default(CancellationToken))
        {
            var booking = await _context.Accounts.SingleAsync(t => t.AccountId == accountId, ct);
            _context.Remove(booking);
            await _context.SaveChangesAsync(ct);
        }

        /// <summary>
        /// Get the item, we default to the snapshot version, unless you return a current version of state
        /// </summary>
        /// <param name="accountId">The id of the account to load</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>THe matching booking</returns>
        public async Task<AccountCardDetails> GetAsync(Guid accountId, CancellationToken ct = default(CancellationToken))
        {
            return await _context.Accounts.SingleAsync(t => t.AccountId == accountId, ct);
        }

        /// <summary>
        /// Save the item
        /// </summary>
        /// <param name="account">The account to save</param>
        /// <param name="ct">Cancel the operataion</param>
        public async Task SaveAsync(AccountCardDetails account, CancellationToken ct = default(CancellationToken))
        {
            await _context.SaveChangesAsync(ct);
        }
     }
}
