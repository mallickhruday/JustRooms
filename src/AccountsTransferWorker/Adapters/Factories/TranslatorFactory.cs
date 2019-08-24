using System;
using AccountsTransferWorker.Adapters.Data;
using AccountsTransferWorker.Ports;
using Paramore.Brighter;

namespace AccountsTransferWorker.Adapters.Factories
{
    public class TranslatorFactory : IRecordTranslatorFactory
    {
        public dynamic Create<TIn, TOut>(Type factory) where TOut : IRequest, new()
        {
            //we only have the one here
            return new AccountFromRecordTranslator();
        }
    }
}