using System;
using System.Threading;
using System.Threading.Tasks;
using DirectBooking.application;
using DirectBooking.ports.repositories;
using Microsoft.EntityFrameworkCore;

namespace DirectBooking.adapters.data
{
    /// <summary>
    /// A unit of work for Entity Framework 
    /// </summary>
    public class EFUnitOfWork : IUnitOfWork
    {
        private BookingContext _context;

        /// <summary>
        /// Construct a unit of work 
        /// </summary>
        /// <param name="context">The EF Context to use</param>
        public EFUnitOfWork(BookingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Delete the item, by default the snapshot version, which deletes the live record, but keeps history
        /// </summary>
        /// <param name="bookingId">The booking to delete</param>
        /// <param name="ct">Cancel the operation</param>
        public async Task DeleteAsync(Guid bookingId, CancellationToken ct = default(CancellationToken))
        {
            var booking = await _context.Bookings.SingleAsync(t => t.RoomBookingId == bookingId, ct);
            _context.Remove(booking);
            await _context.SaveChangesAsync(ct);
        }

        /// <summary>
        /// Get the item, we default to the snapshot version, unless you return a current version of state
        /// </summary>
        /// <param name="bookingId">The id of the booking to load</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>THe matching booking</returns>
        public async Task<RoomBooking> GetAsync(Guid bookingId, CancellationToken ct = default(CancellationToken))
        {
            return await _context.Bookings.SingleAsync(t => t.RoomBookingId == bookingId, ct);
        }

        /// <summary>
        /// Save the item
        /// </summary>
        /// <param name="booking">The booking to save</param>
        /// <param name="ct">Cancel the operataion</param>
        public async Task SaveAsync(RoomBooking booking, CancellationToken ct = default(CancellationToken))
        {
            await _context.SaveChangesAsync(ct);
        }
     }
}