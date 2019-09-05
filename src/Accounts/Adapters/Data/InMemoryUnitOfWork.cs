using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Application;
using Accounts.Ports.Repositories;

namespace Accounts.Adapters.Data
{
    /// <summary>
    /// An in-memory unit of work, for use in testing
    /// </summary>
    public class InMemoryUnitOfWork : IUnitOfWork
    {
        private Dictionary<Item, Account> _accounts = new Dictionary<Item, Account>();
        
       /// <summary>
       /// Remove a guest account version, by default the snapshot which deletes the 'live' account but preserves history
       /// </summary>
       /// <param name="accountId">The account to mark as deleted</param>
       /// <param name="version">The version of the guest account to remove, defaults to the current snapshot which deletes the account</param>
       /// <param name="ct">Token to allow cancelling the ongoing operation</param>
        public Task DeleteAsync(Guid accountId, string version = Account.SnapShot, CancellationToken ct = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<object>();
            var item = new Item(accountId, version);
            if (_accounts.ContainsKey(item))
            {
                _accounts.Remove(item);
            }
            
            tcs.SetResult(new object());
            return tcs.Task;
        }

       /// <summary>
       /// Get an account by id (and version number)
       /// </summary>
       /// <param name="accountId">The id of the account to retrieve</param>
       /// <param name="version">The version to retrieve, by default it is the snapshot i.e. current live record</param>
       /// <param name="ct">Token to allow cancelling the ongoing operation</param>
        public Task<Account> GetAsync(Guid accountId, string version = Account.SnapShot, CancellationToken ct = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<Account>();
            _accounts.TryGetValue(new Item(accountId, version), out Account value);
            tcs.SetResult(value);
            return tcs.Task;
         }

       
        /// <summary>
        /// Save the account record
        /// </summary>
        /// <param name="account">The account to save</param>
        /// <param name="ct">Token to allow cancelling the ongoing operation</param>
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