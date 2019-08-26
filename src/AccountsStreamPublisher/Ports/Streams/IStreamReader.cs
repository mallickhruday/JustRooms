using System.Threading;
using System.Threading.Tasks;

namespace AccountsTransferWorker.Ports.Streams
{
    public interface IStreamReader
    {
        Task InitialiseReader(CancellationToken cancellationToken);
        Task ReadStream(CancellationToken stoppingToken);
    }
}