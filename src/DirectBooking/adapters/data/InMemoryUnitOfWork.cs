using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DirectBooking.application;
using DirectBooking.ports.repositories;

namespace DirectBooking.adapters.data
{
    /// <summary>
    /// An in-memory unit of work, for use in testing
    /// </summary>
    public class InMemoryUnitOfWork : IUnitOfWork
    {
        private Dictionary<Item, RoomBooking> _bookings = new Dictionary<Item, RoomBooking>();
        
       /// <summary>
       /// Remove a guest booking version, by default the snapshot which deletes the 'live' booking but preserves history
       /// </summary>
       /// <param name="bookingId">The booking to mark as deleted</param>
       /// <param name="version">The version of the guest booking to remove, defaults to the current snapshot which deletes the booking</param>
       /// <param name="ct">Token to allow cancelling the ongoing operation</param>
        public Task DeleteAsync(Guid bookingId, string version = RoomBooking.SnapShot, CancellationToken ct = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<object>();
            var item = new Item(bookingId, version);
            if (_bookings.ContainsKey(item))
            {
                _bookings.Remove(item);
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
        public Task<RoomBooking> GetAsync(Guid bookingId, string version = RoomBooking.SnapShot, CancellationToken ct = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<RoomBooking>();
            _bookings.TryGetValue(new Item(bookingId, version), out RoomBooking value);
            tcs.SetResult(value);
            return tcs.Task;
         }

       
        /// <summary>
        /// Save the booking record
        /// </summary>
        /// <param name="booking">The booking to save</param>
        /// <param name="ct">Token to allow cancelling the ongoing operation</param>
        public Task SaveAsync(RoomBooking booking, CancellationToken ct = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<object>();
            
            var key = new Item( Guid.Parse(booking.BookingId), booking.Version);
            if ( _bookings.ContainsKey(key))
            {
                _bookings.Remove(key);
            }

            _bookings.Add(key, booking);
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