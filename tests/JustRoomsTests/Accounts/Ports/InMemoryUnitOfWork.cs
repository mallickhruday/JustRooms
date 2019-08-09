using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Adapters.Data;
using Accounts.Application;
using Accounts.Ports.Repositories;

namespace JustRoomsTests.Accounts.Ports
{
    public class InMemoryUnitOfWork : IUnitOfWork
    {
        private Dictionary<Item, Account> _accounts = new Dictionary<Item, Account>();
        
        public Task ClearAsync(Guid accountId, CancellationToken ct = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<object>();
            var item = new Item(accountId, Account.SnapShot);
            if (_accounts.ContainsKey(item))
            {
                _accounts.Remove(item);
            }
            
            tcs.SetResult(new object());
            return tcs.Task;
        }
        
        public Task DeleteAsync(Guid accountId, CancellationToken ct = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<object>();

            var purgedAccounts = new Dictionary<Item, Account>();
            foreach (var entry in _accounts)
            {
                if (entry.Key.Id != accountId)
                {
                    purgedAccounts.Add(entry.Key, entry.Value);
                }
            }

            _accounts = purgedAccounts;
            
            tcs.SetResult(new object());
            return tcs.Task;
        }

        public Task<Account> GetAsync(Guid accountId, string version = Account.SnapShot, CancellationToken ct = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<Account>();
            _accounts.TryGetValue(new Item(accountId, version), out Account value);
            tcs.SetResult(value);
            return tcs.Task;
         }

       
        public Task SaveAsync(Account account, CancellationToken ct = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<object>();
            
            var key = new Item( Guid.Parse(account.AccountId), account.Version);
            if ( _accounts.ContainsKey(key))
            {
                _accounts.Remove(key);
            }

            _accounts.Add(key, account);
            tcs.SetResult(new object());
            return tcs.Task;
        }


        class Item : IEquatable<Item>
        {
            public Item(Guid id, string version)
            {
                Id = id;
                Version = version;
            }
            
            public Guid Id { get;}
            public string Version { get;}
            
            
            public bool Equals(Item other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Id.Equals(other.Id) && string.Equals(Version, other.Version);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Item) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Id.GetHashCode() * 397) ^ (Version != null ? Version.GetHashCode() : 0);
                }
            }

            public static bool operator ==(Item left, Item right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(Item left, Item right)
            {
                return !Equals(left, right);
            }

       }

     }
}