using AccountsTransferWorker.Application;
using AccountsTransferWorker.Ports;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace AccountsTransferWorker.Adapters.Data
{
    public class DynamoDbRecordProcessor : IRecordProcessor<StreamRecord>
    {
        private readonly RecordTranslatorRegistry _translatorRegistry;
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly ILogger<DynamoDbRecordProcessor> _logger;

        public DynamoDbRecordProcessor(
            RecordTranslatorRegistry translatorRegistry,
            IAmACommandProcessor commandProcessor, 
            ILogger<DynamoDbRecordProcessor> logger)
        {
            _translatorRegistry = translatorRegistry;
            _commandProcessor = commandProcessor;
            _logger = logger;
        }
        
        public void ProcessRecord(StreamRecord streamRecord)
        {
            IRecordTranslator<StreamRecord, AccountEvent> translator = _translatorRegistry.Get<StreamRecord, AccountEvent>();
            var accountEvent = translator.TranslateFromRecord(streamRecord);
            _commandProcessor.Post(accountEvent);
            
            _logger.LogDebug($"Process Record {streamRecord.SequenceNumber}");
        }
    }
}