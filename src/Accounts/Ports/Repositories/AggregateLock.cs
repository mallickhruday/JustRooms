using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Application;

namespace Accounts.Ports.Repositories
{
    public class AggregateLock 
    {
        private readonly string _accountId;
        private readonly string _lockedBy;
        private readonly DateTime _expiresAt;
        private readonly IUnitOfWork _unitOfWork;

        public AggregateLock(string accountId, string lockedBy, DateTime expiresAt, IUnitOfWork unitOfWork)
        {
            _accountId = accountId;
            _lockedBy = lockedBy;
            _expiresAt = expiresAt;
            _unitOfWork = unitOfWork;
        }

        public async Task ReleaseAsync(CancellationToken ct = default(CancellationToken))
        {
            var snapshot = await _unitOfWork.GetAsync(Guid.Parse(_accountId), Account.SnapShot, ct);
            if (snapshot.LockedBy == _lockedBy &&
                snapshot.LockExpiresAt == _expiresAt.ToString(CultureInfo.InvariantCulture))
            {
                snapshot.LockedBy = null;
                snapshot.LockExpiresAt = null;
                await _unitOfWork.SaveAsync(snapshot, ct);
            }
        }
    }
}