using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using CreditCardsAccountStreamReader.Application;
using CreditCardsAccountStreamReader.Ports.Repositories;

namespace CreditCardsAccountStreamReader.Adapters.Data
{
    public class DynamoDbUnitOfWork : IUnitOfWork
    {
        private DynamoDBContext _context;

        public DynamoDbUnitOfWork(IAmazonDynamoDB amazonDynamoDb)
        {
            _context = new DynamoDBContext(amazonDynamoDb);
        }
       
        /// <summary>
        /// Delete the item, all versions
        /// </summary>
        /// <param name="accountId">The account to delete</param>
        /// <param name="ct">Cancel the operation</param>
        public async Task DeleteAsync(Guid accountId, string version, CancellationToken ct = default(CancellationToken))
        {
            await _context.DeleteAsync<AccountCardDetails>(accountId.ToString(), version, ct);
        }

        /// <summary>
        /// Get the item, we default to the snapshot version, unless you return a current version of state
        /// </summary>
        /// <param name="accountId">The id of the account to load</param>
        /// <param name="version">The version number to load</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>THe matching account</returns>
        public async Task<AccountCardDetails> GetAsync(Guid accountId, string version = AccountCardDetails.SnapShot, CancellationToken ct = default(CancellationToken))
        {
            return await _context.LoadAsync<AccountCardDetails>(accountId.ToString(), version, ct);
        }

        /// <summary>
        /// Save the item
        /// </summary>
        /// <param name="account">The account to save</param>
        /// <param name="ct">Cancel the operataion</param>
        public async Task SaveAsync(AccountCardDetails account, CancellationToken ct = default(CancellationToken))
        {
            await _context.SaveAsync(account, ct).ConfigureAwait(false);
        }
     }
}