using System;
using Paramore.Brighter;

namespace AccountsTransferWorker.Ports.Streams
{
    public interface IRecordTranslatorFactory
    {
        dynamic Create<TIn, TOut>(Type factory) where TOut : IRequest, new();
    }
}