using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CreditCardCore.Application;
using CreditCardCore.Ports.Repositories;

namespace CreditCardCore.Adapters.Data
{
    public class InMemoryUnitOfWork : IUnitOfWork
    {
        private Dictionary<Guid, AccountCardDetails> _bookings = new Dictionary<Guid, AccountCardDetails>();

        /// <summary>
        /// Remove a guest booking version, by default the snapshot which deletes the 'live' booking but preserves history
        /// </summary>
        /// <param name="bookingId">The booking to mark as deleted</param>
        /// <param name="version">The version of the guest booking to remove, defaults to the current snapshot which deletes the booking</param>
        /// <param name="ct">Token to allow cancelling the ongoing operation</param>
        public Task DeleteAsync(Guid bookingId, CancellationToken ct = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<object>();
            if (_bookings.ContainsKey(bookingId))
            {
                _bookings.Remove(bookingId);
            }

            tcs.SetResult(new object());
            return tcs.Task;
        }

        /// <summary>
        /// Get an booking by id (and version number)
        /// </summary>
        /// <param name="bookingId">The id of the booking to retrieve</param>
        /// <param name="version">The version to retrieve, by default it is the snapshot i.e. current live record</param>
        /// <param name="ct">Token to allow cancelling the ongoing operation</param>
        public Task<AccountCardDetails> GetAsync(Guid bookingId, CancellationToken ct = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<AccountCardDetails>();
            _bookings.TryGetValue(bookingId, out AccountCardDetails value);
            tcs.SetResult(value);
            return tcs.Task;
        }


        /// <summary>
        /// Save the booking record
        /// </summary>
        /// <param name="booking">The booking to save</param>
        /// <param name="ct">Token to allow cancelling the ongoing operation</param>
        public Task SaveAsync(AccountCardDetails booking, CancellationToken ct = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<object>();

            var key = booking.AccountId;
            if (_bookings.ContainsKey(key))
            {
                _bookings.Remove(key);
            }

            _bookings.Add(key, booking);
            tcs.SetResult(new object());
            return tcs.Task;
        }
    }
}