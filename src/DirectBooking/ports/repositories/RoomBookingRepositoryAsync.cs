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
        public async Task AddAsync(RoomBooking roomBooking, CancellationToken ct = default(CancellationToken))
        {
            
            //set incoming record to V1
            roomBooking.CurrentVersion = 1; 
            roomBooking.Version = RoomBooking.VersionPrefix + $"{roomBooking.CurrentVersion}";
            
            //copy the new roomBooking record into version 0
            var snapshot = new RoomBooking(
                roomBooking.BookingId,
                roomBooking.DateOfFirstNight,
                roomBooking.NumberOfNights,
                roomBooking.NumberOfGuests,
                roomBooking.RoomType,
                roomBooking.Price,
                roomBooking.AccountId);
            snapshot.CurrentVersion = roomBooking.CurrentVersion;
            snapshot.Version = RoomBooking.SnapShot;
            
            //save snapshot and history
            await _unitOfWork.SaveAsync(snapshot, ct);
            await _unitOfWork.SaveAsync(roomBooking, ct);
        }

         /// <summary>
        /// Delete the roomBooking by removing the 'current record' but leave history
        /// </summary>
        /// <param name="bookingId">The id of the roomBooking to remove the snapshot of</param>
        /// <param name="ct">Operation cancellation</param>
        public async Task DeleteAsync(Guid bookingId, CancellationToken ct = default(CancellationToken))
        {
            await _unitOfWork.DeleteAsync(bookingId, RoomBooking.SnapShot, ct);
        }
        
        /// <summary>
        /// Get the current record
        /// </summary>
        /// <param name="bookingId">The roomBooking to get</param>
        /// <param name="ct">Operation cancellation</param>
        /// <returns>The roomBooking matching the Id, or null</returns>
        public async Task<RoomBooking> GetAsync(Guid bookingId, CancellationToken ct = default(CancellationToken))
        {
            return await _unitOfWork.GetAsync(bookingId, RoomBooking.SnapShot, ct);
        }

        /// <summary>
        /// Get a specific version of an roomBooking
        /// </summary>
        /// <param name="bookingId">The roomBooking to get</param>
        /// <param name="version">The version of the roomBooking expressed as "VN" where V0 is the snapshot </param>
        /// <param name="ct"></param>
        /// <returns>The matching version of the roomBooking</returns>
         public async Task<RoomBooking> GetAsync(Guid bookingId, string version, CancellationToken ct = default(CancellationToken))
        {
            return await _unitOfWork.GetAsync(bookingId, version, ct);
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

            await _unitOfWork.SaveAsync(snapshot, ct);
            
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
            var snapshot = await _unitOfWork.GetAsync(Guid.Parse(newBookingVersion.AccountId), ct: ct);
            var nextVersion = snapshot.CurrentVersion + 1;
            newBookingVersion.CurrentVersion = nextVersion;
            newBookingVersion.Version = RoomBooking.VersionPrefix + $"{nextVersion}";

            //create a new snapshot from the current record
            var newSnapshot = new RoomBooking(
                newBookingVersion.BookingId,
                 newBookingVersion.DateOfFirstNight,
                newBookingVersion.NumberOfNights,
                newBookingVersion.NumberOfGuests,
                newBookingVersion.RoomType,
                newBookingVersion.Price,
                newBookingVersion.AccountId);
            newSnapshot.CurrentVersion = nextVersion;
            newSnapshot.Version = RoomBooking.SnapShot;

            //save the snapshot and the new version
            await _unitOfWork.SaveAsync(newSnapshot, ct);
            await _unitOfWork.SaveAsync(newBookingVersion, ct);
 
        }


    }
}