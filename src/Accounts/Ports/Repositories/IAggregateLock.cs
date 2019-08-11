using System.Threading;
using System.Threading.Tasks;

namespace Accounts.Ports.Repositories
{
    public interface IAggregateLock
    {
        Task ReleaseAsync(CancellationToken ct = default(CancellationToken));
    }
}