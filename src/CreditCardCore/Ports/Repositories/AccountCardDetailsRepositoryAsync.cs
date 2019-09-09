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
        


         /// <summary>
        /// Delete the cardDetails by removing the 'current record' but leave history
        /// </summary>
        /// <param name="accountId">The id of the cardDetails to remove the snapshot of</param>
        /// <param name="ct">Operation cancellation</param>
        public async Task DeleteAsync(Guid accountId, CancellationToken ct = default(CancellationToken))
        {
            await _unitOfWork.DeleteAsync(accountId, AccountCardDetails.SnapShot, ct);
        }
        
        /// <summary>
        /// Get the current record
        /// </summary>
        /// <param name="accountId">The cardDetails to get</param>
        /// <param name="ct">Operation cancellation</param>
        /// <returns>The cardDetails matching the Id, or null</returns>
        public async Task<AccountCardDetails> GetAsync(Guid accountId, CancellationToken ct = default(CancellationToken))
        {
            return await _unitOfWork.GetAsync(accountId, AccountCardDetails.SnapShot, ct);
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
            return await _unitOfWork.GetAsync(accountId, version, ct);
        }

        public async Task UpsertAsync(AccountCardDetails cardDetails, CancellationToken ct = default(CancellationToken))
        {
            //if it exists
            var existingDetails = await GetAsync(Guid.Parse(cardDetails.AccountId), ct);
            if (existingDetails != null)
            { 
                //if the version is earlier than the version we have
                if (cardDetails.CurrentVersion > existingDetails.CurrentVersion)
                {
                    AggregateLock aggregateLock = null;
                    try
                    {
                        //lock it
                        aggregateLock = await LockAsync(cardDetails.AccountId, "SYS", ct);

                        //update it
                        await UpdateAsync(cardDetails, aggregateLock, ct);
                    }
                    finally
                    {
                        if (aggregateLock != null)
                            await aggregateLock.ReleaseAsync();
                    }

                }
            }
            else
            {
                //add it
                await AddAsync(cardDetails, ct);
            }
        }
        

        private async Task AddAsync(AccountCardDetails cardDetails, CancellationToken ct = default(CancellationToken))
        {
            
            //set incoming record to V1
            cardDetails.CurrentVersion = 1; 
            cardDetails.Version = AccountCardDetails.VersionPrefix + $"{cardDetails.CurrentVersion}";
            
            //copy the new cardDetails record into version 0
            var snapshot = new AccountCardDetails(
                cardDetails.AccountId, 
                cardDetails.Name, 
                cardDetails.CardNumber, cardDetails.CardSecurityCode, cardDetails.FirstLineOfAddress, cardDetails.ZipCode);
            snapshot.CurrentVersion = cardDetails.CurrentVersion;
            snapshot.Version = AccountCardDetails.SnapShot;
            
            //save snapshot and history
            await _unitOfWork.SaveAsync(snapshot, ct);
            await _unitOfWork.SaveAsync(cardDetails, ct);
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

            await _unitOfWork.SaveAsync(snapshot, ct);
            
            return new AggregateLock(accountId, whoIsLocking, expiresAt, _unitOfWork);
        }
        
        private async Task UpdateAsync(AccountCardDetails cardDetails, AggregateLock aggregateLock, CancellationToken ct = default(CancellationToken))
        {
            //find the next version to use
            var snapshot = await _unitOfWork.GetAsync(Guid.Parse(cardDetails.AccountId), ct: ct);
            var nextVersion = snapshot.CurrentVersion + 1;
            cardDetails.CurrentVersion = nextVersion;
            cardDetails.Version = AccountCardDetails.VersionPrefix + $"{nextVersion}";

            //create a new snapshot from the current record
             var newSnapshot = new AccountCardDetails(
                cardDetails.AccountId, 
                cardDetails.Name, 
                cardDetails.CardNumber, cardDetails.CardSecurityCode, cardDetails.FirstLineOfAddress, cardDetails.ZipCode);
            newSnapshot.CurrentVersion = nextVersion;
            newSnapshot.Version = AccountCardDetails.SnapShot;

            //save the snapshot and the new version
            await _unitOfWork.SaveAsync(newSnapshot, ct);
            await _unitOfWork.SaveAsync(cardDetails, ct);
 
        }


    }
}