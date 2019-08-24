using Amazon.DynamoDBv2.Model;

namespace AccountsTransferWorker.Ports
{
    public interface IRecordProcessor<TIn>
    {
        void ProcessRecord(TIn streamRecord);
    }
}