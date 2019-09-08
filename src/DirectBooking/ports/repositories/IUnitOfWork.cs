using System;
using System.Threading;
using System.Threading.Tasks;
using DirectBooking.application;

namespace DirectBooking.ports.repositories
{
    /// <summary>
    /// An abstraction over a storage layer
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Delete the item, all versions
        /// </summary>
        /// <param name="bookingId">The booking to delete</param>
        /// <param name="version">The version to delete, defaults to the snapshot version, deleting visibility but preserving history</param>
        /// <param name="ct">Cancel the operation</param>
        Task DeleteAsync(Guid bookingId, string version = RoomBooking.SnapShot, CancellationToken ct = default(CancellationToken));
         
        /// <summary>
        /// Get the item, we default to the snapshot version, unless you return a current version of state
        /// </summary>
        /// <param name="accountId">The id of the booking to load</param>
        /// <param name="version">The version number to load</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>THe matching account</returns>
        Task<RoomBooking> GetAsync(Guid bookingId, string version = RoomBooking.SnapShot, CancellationToken ct = default(CancellationToken));

        /// <summary> 
        /// Save the item
        /// </summary>
        /// <param name="roomBooking">The room booking to save</param>
        /// <param name="ct">Cancel the operataion</param>
        Task SaveAsync(RoomBooking booking, CancellationToken ct = default(CancellationToken));
        
   }
}