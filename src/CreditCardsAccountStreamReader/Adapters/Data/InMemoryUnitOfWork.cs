using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CreditCardsAccountStreamReader.Application;
using CreditCardsAccountStreamReader.Ports.Repositories;

namespace CreditCardsAccountStreamReader.Adapters.Data
{
    public class InMemoryUnitOfWork : IUnitOfWork
    {
        private Dictionary<Item, AccountCardDetails> _cardDetails = new Dictionary<Item, AccountCardDetails>();
        
        public Task DeleteAsync(Guid accountId, string version = AccountCardDetails.SnapShot, CancellationToken ct = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<object>();
            var item = new Item(accountId, version);
            if (_cardDetails.ContainsKey(item))
            {
                _cardDetails.Remove(item);
            }
            
            tcs.SetResult(new object());
            return tcs.Task;
        }

        public Task<AccountCardDetails> GetAsync(Guid accountId, string version = AccountCardDetails.SnapShot, CancellationToken ct = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<AccountCardDetails>();
            _cardDetails.TryGetValue(new Item(accountId, version), out AccountCardDetails value);
            tcs.SetResult(value);
            return tcs.Task;
         }

       
        public Task SaveAsync(AccountCardDetails account, CancellationToken ct = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<object>();
            
            var key = new Item( Guid.Parse(account.AccountId), account.Version);
            if ( _cardDetails.ContainsKey(key))
            {
                _cardDetails.Remove(key);
            }

            _cardDetails.Add(key, account);
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