using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using DirectBooking.application;
using DirectBooking.ports.Exceptions;

namespace DirectBooking.ports.repositories
{
    /// <summary>
    /// Manages a collection of guest accounts
    /// </summary>
    public class RoomBookingRepositoryAsync 
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructs a guest roomBooking repository
        /// </summary>
        public RoomBookingRepositoryAsync(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        /// <summary>
        /// Adds a new current record to the database and a version 1 record
        /// </summary>
        /// <param name="roomBooking">The roomBooking to add</param>
        /// <param name="ct">Operation cancellation</param>
        public async Task AddAsync(RoomBooking booking, CancellationToken ct = default(CancellationToken))
        {
            await _unitOfWork.AddAsync(booking, ct);
        }

         /// <summary>
        /// Delete the roomBooking by removing the 'current record' but leave history
        /// </summary>
        /// <param name="bookingId">The id of the roomBooking to remove the snapshot of</param>
        /// <param name="ct">Operation cancellation</param>
        public async Task DeleteAsync(Guid bookingId, CancellationToken ct = default(CancellationToken))
        {
            await _unitOfWork.DeleteAsync(bookingId, ct);
        }
        
        /// <summary>
        /// Get the current record
        /// </summary>
        /// <param name="bookingId">The roomBooking to get</param>
        /// <param name="ct">Operation cancellation</param>
        /// <returns>The roomBooking matching the Id, or null</returns>
        public async Task<RoomBooking> GetAsync(Guid bookingId, CancellationToken ct = default(CancellationToken))
        {
            try
            {
                return await _unitOfWork.GetAsync(bookingId, ct);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        /// <summary>
        /// Lock the current record for 500ms whilst we edit it
        /// </summary>
        /// <param name="bookingId">The roomBooking to lock</param>
        /// <param name="whoIsLocking"></param>
        /// <param name="ct">Operation Cancellation</param>
        /// <returns>Someone else holds a lock</returns>
        public async Task<AggregateLock> LockAsync(string bookingId, string whoIsLocking, CancellationToken ct = default(CancellationToken))
        {
            var snapshot = await GetAsync(Guid.Parse(bookingId), ct);
            if (snapshot.LockedBy != null && (snapshot.LockedBy != whoIsLocking || !(DateTime.UtcNow > DateTime.Parse(snapshot.LockExpiresAt))))
            {
                throw new CannotGetLockException($"Booking {bookingId} is locked by {snapshot.LockedBy}");
            }

            snapshot.LockedBy = whoIsLocking;
            var expiresAt = DateTime.UtcNow.AddMilliseconds(500);
            snapshot.LockExpiresAt = expiresAt.ToString(CultureInfo.InvariantCulture);

            await _unitOfWork.UpdateAsync(ct);
            
            return new AggregateLock(bookingId, whoIsLocking, expiresAt, _unitOfWork);
        }

        /// <summary>
        /// Update an roomBooking
        /// </summary>
        /// <param name="newBookingVersion">The new roomBooking version</param>
        /// <param name="aggregateLock">A pessimistic lock, retrieved from a Lock call, that allows us to update this guest roomBooking</param>
        /// <param name="ct">A token for operation cancellation</param>
        public async Task UpdateAsync(RoomBooking newBookingVersion, AggregateLock aggregateLock, CancellationToken ct = default(CancellationToken))
        {
            //find the next version to use
           await _unitOfWork.UpdateAsync(ct);
        }


    }
}