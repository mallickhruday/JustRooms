using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using DirectBooking.application;
using DirectBooking.ports.repositories;

namespace DirectBooking.adapters.data
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
        /// <param name="bookingId">The booking to delete</param>
        /// <param name="version">The version of the booking to delete</param>
        /// <param name="ct">Cancel the operation</param>
        public async Task DeleteAsync(Guid bookingId, string version = RoomBooking.SnapShot, CancellationToken ct = default(CancellationToken))
        {
            await _context.DeleteAsync<RoomBooking>(bookingId.ToString(), version, ct);
        }

        /// <summary>
        /// Get the item, we default to the snapshot version, unless you return a current version of state
        /// </summary>
        /// <param name="bookingId">The id of the booking to load</param>
        /// <param name="version">The version number to load</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>THe matching booking</returns>
        public async Task<RoomBooking> GetAsync(Guid bookingId, string version = RoomBooking.SnapShot, CancellationToken ct = default(CancellationToken))
        {
            return await _context.LoadAsync<RoomBooking>(bookingId.ToString(), version, ct);
        }

        /// <summary>
        /// Save the item
        /// </summary>
        /// <param name="booking">The booking to save</param>
        /// <param name="ct">Cancel the operataion</param>
        public async Task SaveAsync(RoomBooking booking, CancellationToken ct = default(CancellationToken))
        {
            await _context.SaveAsync(booking, ct).ConfigureAwait(false);
        }
     }
}