using System;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Application;
using Accounts.Ports.Repositories;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace Accounts.Adapters.Data
{
    /// <summary>
    /// A unit of work for AWS Dynamo Db
    /// </summary>
    public class DynamoDbUnitOfWork : IUnitOfWork
    {
        private DynamoDBContext _context;

        /// <summary>
        /// Construct a unit of work from an AWS Dynamo client
        /// </summary>
        /// <param name="amazonDynamoDb">An AWS Dynamo DB Client instance</param>
        public DynamoDbUnitOfWork(IAmazonDynamoDB amazonDynamoDb)
        {
            _context = new DynamoDBContext(amazonDynamoDb);
        }

        /// <summary>
        /// Delete the item, by default the snapshot version, which deletes the live record, but keeps history
        /// </summary>
        /// <param name="accountId">The account to delete</param>
        /// <param name="version">The version of the account to delete</param>
        /// <param name="ct">Cancel the operation</param>
        public async Task DeleteAsync(Guid accountId, string version = Account.SnapShot, CancellationToken ct = default(CancellationToken))
        {
            await _context.DeleteAsync<Account>(accountId.ToString(), version, ct);
        }

        /// <summary>
        /// Get the item, we default to the snapshot version, unless you return a current version of state
        /// </summary>
        /// <param name="accountId">The id of the account to load</param>
        /// <param name="version">The version number to load</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>THe matching account</returns>
        public async Task<Account> GetAsync(Guid accountId, string version = Account.SnapShot, CancellationToken ct = default(CancellationToken))
        {
            return await _context.LoadAsync<Account>(accountId.ToString(), version, ct);
        }

        /// <summary>
        /// Save the item
        /// </summary>
        /// <param name="account">The account to save</param>
        /// <param name="ct">Cancel the operataion</param>
        public async Task SaveAsync(Account account, CancellationToken ct = default(CancellationToken))
        {
            await _context.SaveAsync(account, ct).ConfigureAwait(false);
        }
     }
}