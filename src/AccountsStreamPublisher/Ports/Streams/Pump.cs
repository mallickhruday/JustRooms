using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Paramore.Brighter;
using Polly;
using Polly.Registry;

namespace AccountsTransferWorker.Ports.Streams
{
    public class Pump : BackgroundService
    {
       private readonly IReadOnlyPolicyRegistry<string> _policyRegistry;
       private readonly IStreamReader _streamReader;

       public Pump(
            IReadOnlyPolicyRegistry<string> policyRegistry,
            IStreamReader streamReader
        )
        {
            _policyRegistry = policyRegistry;
            _streamReader = streamReader;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
           await  _streamReader.InitialiseReader(cancellationToken);
           await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var policyWrap = Policy.WrapAsync(
                _policyRegistry.Get<IAsyncPolicy>(CommandProcessor.CIRCUITBREAKERASYNC),
                _policyRegistry.Get<IAsyncPolicy>(CommandProcessor.RETRYPOLICYASYNC)
                );

            await policyWrap.ExecuteAsync(async () => { await _streamReader.ReadStream(stoppingToken); });
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Stopping pump");
            await base.StopAsync(cancellationToken);
        }
    }
}