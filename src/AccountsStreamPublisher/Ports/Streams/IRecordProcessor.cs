namespace AccountsTransferWorker.Ports.Streams
{
    public interface IRecordProcessor<TIn>
    {
        void ProcessRecord(TIn streamRecord);
    }
}