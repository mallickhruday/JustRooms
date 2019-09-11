using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using DirectBooking.application;

namespace DirectBooking.ports.repositories
{
    /// <summary>
    /// A pessimistic lock on an account record
    /// </summary>
    public class AggregateLock 
    {
        private readonly string _bookingId;
        private readonly string _lockedBy;
        private readonly DateTime _expiresAt;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructs an aggregate lock - don't use this directly, ask a repository to give you a lock
        /// </summary>
        /// <param name="bookingId">The account that has been locked</param>
        /// <param name="lockedBy">Who has locked the account</param>
        /// <param name="expiresAt">When does the lock expire</param>
        /// <param name="unitOfWork">A unit of work over the storage of accounts</param>
        public AggregateLock(string bookingId, string lockedBy, DateTime expiresAt, IUnitOfWork unitOfWork)
        {
            _bookingId = bookingId;
            _lockedBy = lockedBy;
            _expiresAt = expiresAt;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Releases the lock on the account
        /// </summary>
        /// <param name="ct">A token that allows the operation to be cancelled</param>
        /// <returns></returns>
        public async Task ReleaseAsync(CancellationToken ct = default(CancellationToken))
        {
            var snapshot = await _unitOfWork.GetAsync(Guid.Parse(_bookingId),ct);
            if (snapshot.LockedBy == _lockedBy &&
                snapshot.LockExpiresAt == _expiresAt.ToString(CultureInfo.InvariantCulture))
            {
                snapshot.LockedBy = null;
                snapshot.LockExpiresAt = null;
                await _unitOfWork.UpdateAsync(ct);
            }
        }
    }
}