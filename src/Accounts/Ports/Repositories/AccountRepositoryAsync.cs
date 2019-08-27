using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Adapters.Data;
using Accounts.Application;
using Accounts.Ports.Exceptions;

namespace Accounts.Ports.Repositories
{
    public class AccountRepositoryAsync 
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountRepositoryAsync(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        /// <summary>
        /// Adds a new current record to the database and a version 1 record
        /// </summary>
        /// <param name="account">The account to add</param>
        /// <param name="ct">Operation cancellation</param>
        public async Task AddAsync(Account account, CancellationToken ct = default(CancellationToken))
        {
            
            //set incoming record to V1
            account.CurrentVersion = 1; 
            account.Version = Account.VersionPrefix + $"{account.CurrentVersion}";
            
            //copy the new account record into version 0
            var snapshot = new Account(Guid.Parse(account.AccountId), account.Name, account.Addresses, account.ContactDetails, account.CardDetails);
            snapshot.CurrentVersion = account.CurrentVersion;
            snapshot.Version = Account.SnapShot;
            
            //save snapshot and history
            await _unitOfWork.SaveAsync(snapshot, ct);
            await _unitOfWork.SaveAsync(account, ct);
        }

         /// <summary>
        /// Delete the account by removing the 'current record' but leave history
        /// </summary>
        /// <param name="accountId">The id of the account to remove the snapshot of</param>
        /// <param name="ct">Operation cancellation</param>
        public async Task DeleteAsync(Guid accountId, CancellationToken ct = default(CancellationToken))
        {
            await _unitOfWork.DeleteAsync(accountId, Account.SnapShot, ct);
        }
        
        /// <summary>
        /// Get the current record
        /// </summary>
        /// <param name="accountId">The account to get</param>
        /// <param name="ct">Operation cancellation</param>
        /// <returns>The account matching the Id, or null</returns>
        public async Task<Account> GetAsync(Guid accountId, CancellationToken ct = default(CancellationToken))
        {
            return await _unitOfWork.GetAsync(accountId, Account.SnapShot, ct);
        }

        /// <summary>
        /// Get a specific version of an account
        /// </summary>
        /// <param name="accountId">The account to get</param>
        /// <param name="version">The version of the account expressed as "VN" where V0 is the snapshot </param>
        /// <param name="ct"></param>
        /// <returns>The matching version of the account</returns>
         public async Task<Account> GetAsync(Guid accountId, string version, CancellationToken ct = default(CancellationToken))
        {
            return await _unitOfWork.GetAsync(accountId, version, ct);
        }

        /// <summary>
        /// Lock the current record for 500ms whilst we edit it
        /// </summary>
        /// <param name="accountId">The account to lock</param>
        /// <param name="whoIsLocking"></param>
        /// <param name="ct">Operation Cancellation</param>
        /// <returns>Someone else holds a lock</returns>
        public async Task<AggregateLock> LockAsync(string accountId, string whoIsLocking, CancellationToken ct = default(CancellationToken))
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

        /// <summary>
        /// Update an account
        /// </summary>
        /// <param name="account">The updated account</param>
        /// <param name="ct">Operation cancellation</param>
        public async Task UpdateAsync(Account newAccountVersion, AggregateLock aggregateLock, CancellationToken ct = default(CancellationToken))
        {
            //find the next version to use
            var snapshot = await _unitOfWork.GetAsync(Guid.Parse(newAccountVersion.AccountId), ct: ct);
            var nextVersion = snapshot.CurrentVersion + 1;
            newAccountVersion.CurrentVersion = nextVersion;
            newAccountVersion.Version = Account.VersionPrefix + $"{nextVersion}";

            //create a new snapshot from the current record
            var newSnapshot = new Account(Guid.Parse(newAccountVersion.AccountId), newAccountVersion.Name, newAccountVersion.Addresses, newAccountVersion.ContactDetails, newAccountVersion.CardDetails);
            newSnapshot.CurrentVersion = nextVersion;
            newSnapshot.Version = Account.SnapShot;

            //save the snapshot and the new version
            await _unitOfWork.SaveAsync(newSnapshot, ct);
            await _unitOfWork.SaveAsync(newAccountVersion, ct);
 
        }


    }
}