using Paramore.Brighter;

namespace AccountsTransferWorker.Ports.Streams
{
    public interface IRecordTranslator<TIn, TOut> where TOut : IRequest, new()
    {
        TOut TranslateFromRecord(TIn record);
    }
}