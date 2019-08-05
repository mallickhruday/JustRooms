using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Application;
using Accounts.Ports.Repositories;

namespace JustRoomsTests.Accounts.Ports
{
    public class InMemoryAccountRepository : IAccountRepositoryAsync
    {
        private readonly Dictionary<Guid, Account> _accounts = new Dictionary<Guid, Account>();
        
        public Task AddAsync(Account account, CancellationToken ct = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<object>();
            _accounts.Add(Guid.Parse(account.AccountId), account);
            tcs.SetResult(new object());
            return tcs.Task;
        }

        public Task<Account> GetAsync(Guid accountId, CancellationToken ct = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<Account>();
            _accounts.TryGetValue(accountId, out Account value);
            tcs.SetResult(value);
            return tcs.Task;
         }

        public Task UpdateAsync(Account account, CancellationToken ct = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<object>();
            var key = Guid.Parse(account.AccountId);
            if (_accounts.ContainsKey(key))
            {
                _accounts[key] = account;
            }
            
            tcs.SetResult(new object());
            return tcs.Task;
        }

        public Task DeleteAsync(Guid accountId, CancellationToken ct = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<object>();
            if (_accounts.ContainsKey(accountId))
            {
                _accounts.Remove(accountId);
            }
            
            tcs.SetResult(new object());
            return tcs.Task;
        }
    }
}