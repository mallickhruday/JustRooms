using System;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Application;
using Accounts.Ports.Repositories;

namespace Accounts.Adapters.Data
{
    public class AccountRepositoryAsync : IAccountRepositoryAsync
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountRepositoryAsync(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
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

        public async Task DeleteAsync(Guid accountId, CancellationToken ct = default(CancellationToken))
        {
            await _unitOfWork.ClearAsync(accountId, ct);
        }
        
        public async Task<Account> GetAsync(Guid accountId, CancellationToken ct = default(CancellationToken))
        {
            return await _unitOfWork.GetAsync(accountId, Account.SnapShot, ct);
        }

        public async Task<Account> GetAsync(Guid accountId, string version, CancellationToken ct = default(CancellationToken))
        {
            return await _unitOfWork.GetAsync(accountId, version, ct);
        }
        
        public async Task<bool> LockAsync(Guid accountId, CancellationToken ct = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Account newAccountVersion, CancellationToken ct = default(CancellationToken))
        {
            var snapshot = await _unitOfWork.GetAsync(Guid.Parse(newAccountVersion.AccountId), ct: ct);
            var nextVersion = snapshot.CurrentVersion + 1;
            newAccountVersion.CurrentVersion = nextVersion;
            newAccountVersion.Version = Account.VersionPrefix + $"{nextVersion}";

            var newSnapshot = new Account(Guid.Parse(newAccountVersion.AccountId), newAccountVersion.Name, newAccountVersion.Addresses, newAccountVersion.ContactDetails, newAccountVersion.CardDetails);
            newSnapshot.CurrentVersion = nextVersion;
            newSnapshot.Version = Account.SnapShot;

            await _unitOfWork.SaveAsync(newSnapshot, ct);
            await _unitOfWork.SaveAsync(newAccountVersion, ct);
 
        }


    }
}