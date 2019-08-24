using Amazon.DynamoDBv2.Model;
using Paramore.Brighter;

namespace AccountsTransferWorker.Ports
{
    public interface IRecordTranslator<TIn, TOut> where TOut : IRequest, new()
    {
        TOut TranslateFromRecord(TIn record);
    }
}