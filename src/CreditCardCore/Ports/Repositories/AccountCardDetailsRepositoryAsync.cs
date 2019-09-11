using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using CreditCardCore.Application;
using CreditCardCore.Ports.Exceptions;

namespace CreditCardCore.Ports.Repositories
{
    public class AccountCardDetailsRepositoryAsync 
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountCardDetailsRepositoryAsync(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async Task AddAsync(AccountCardDetails cardDetails, CancellationToken ct = default(CancellationToken))
        {
            await _unitOfWork.AddAsync(cardDetails, ct);
        }    

        /// <summary>
        /// Delete the cardDetails by removing the 'current record' but leave history
        /// </summary>
        /// <param name="accountId">The id of the cardDetails to remove the snapshot of</param>
        /// <param name="ct">Operation cancellation</param>
        public async Task DeleteAsync(Guid accountId, CancellationToken ct = default(CancellationToken))
        {
            await _unitOfWork.DeleteAsync(accountId, ct);
        }
        
        /// <summary>
        /// Get the current record
        /// </summary>
        /// <param name="accountId">The cardDetails to get</param>
        /// <param name="ct">Operation cancellation</param>
        /// <returns>The cardDetails matching the Id, or null</returns>
        public async Task<AccountCardDetails> GetAsync(Guid accountId, CancellationToken ct = default(CancellationToken))
        {
            try
            {
                return await _unitOfWork.GetAsync(accountId, ct);
            }
            catch (InvalidOperationException)
            {
                //not in database
                return null;
            }
        }

        /// <summary>
        /// Get a specific version of an cardDetails
        /// </summary>
        /// <param name="accountId">The cardDetails to get</param>
        /// <param name="version">The version of the cardDetails expressed as "VN" where V0 is the snapshot </param>
        /// <param name="ct"></param>
        /// <returns>The matching version of the cardDetails</returns>
         public async Task<AccountCardDetails> GetAsync(Guid accountId, string version, CancellationToken ct = default(CancellationToken))
        {
            return await _unitOfWork.GetAsync(accountId, ct);
        }

        public async Task UpsertAsync(AccountCardDetails cardDetails, CancellationToken ct = default(CancellationToken))
        {
            //if it exists
            var existingDetails = await GetAsync(cardDetails.AccountId, ct);
            if (existingDetails != null)
            { 
                //TODO: Compare byte[] versions
                AggregateLock aggregateLock = null;
                try
                {
                    //lock it
                    aggregateLock = await LockAsync(cardDetails.AccountId.ToString(), "SYS", ct);

                    //update it
                    await UpdateAsync(cardDetails, aggregateLock, ct);
                }
                finally
                {
                    if (aggregateLock != null)
                        await aggregateLock.ReleaseAsync();
                }
            }
            else
            {
                //add it
                await AddAsync(cardDetails, ct);
            }
        }


       private async Task<AggregateLock> LockAsync(string accountId, string whoIsLocking, CancellationToken ct = default(CancellationToken))
        {
            var snapshot = await GetAsync(Guid.Parse(accountId), ct);
            if (snapshot.LockedBy != null && (snapshot.LockedBy != whoIsLocking || !(DateTime.UtcNow > DateTime.Parse(snapshot.LockExpiresAt))))
            {
                throw new CannotGetLockException($"Account Item {accountId} is locked by {snapshot.LockedBy}");
            }

            snapshot.LockedBy = whoIsLocking;
            var expiresAt = DateTime.UtcNow.AddMilliseconds(500);
            snapshot.LockExpiresAt = expiresAt.ToString(CultureInfo.InvariantCulture);

            await _unitOfWork.UpdateAsync(snapshot, ct);
            
            return new AggregateLock(accountId, whoIsLocking, expiresAt, _unitOfWork);
        }
        
        private async Task UpdateAsync(AccountCardDetails cardDetails, AggregateLock aggregateLock, CancellationToken ct = default(CancellationToken))
        {
            await _unitOfWork.AddAsync(cardDetails, ct);
        }
    }
}