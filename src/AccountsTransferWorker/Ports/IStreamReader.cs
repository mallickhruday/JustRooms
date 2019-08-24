using System.Threading;
using System.Threading.Tasks;

namespace AccountsTransferWorker.Ports
{
    public interface IStreamReader
    {
        Task InitialiseReader(CancellationToken cancellationToken);
        Task ReadStream(CancellationToken stoppingToken);
    }
}