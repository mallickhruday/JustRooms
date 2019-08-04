using System;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Application;
using Accounts.Ports.Repositories;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace Accounts.Adapters.Data
{
    public class AccountRepositoryAsync : IAccountRepositoryAsync
    {
        private DynamoDBContext _context;

        public AccountRepositoryAsync(IAmazonDynamoDB amazonDynamoDb)
        {
            _context = new DynamoDBContext(amazonDynamoDb);
        }
        public async Task AddAsync(Account account, CancellationToken ct = default(CancellationToken))
        {
            await _context.SaveAsync(account, ct).ConfigureAwait(false);
        }

        public async Task<Account> GetAsync(Guid accountId, CancellationToken ct = default(CancellationToken))
        {
            return await _context.LoadAsync<Account>(accountId.ToString(), ct);
        }

        public async Task UpdateAsync(Account account, CancellationToken ct = default(CancellationToken))
        {
            await _context.SaveAsync(account, ct).ConfigureAwait(false);
        }

        public async Task DeleteAsync(Guid accountAccountId, CancellationToken ct = default(CancellationToken))
        {
            await _context.DeleteAsync<Account>(accountAccountId.ToString(), ct);
        }
    }
}