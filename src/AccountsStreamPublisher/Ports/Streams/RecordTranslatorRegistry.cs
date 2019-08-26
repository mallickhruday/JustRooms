using System;
using System.Collections.Generic;
using Paramore.Brighter;

namespace AccountsTransferWorker.Ports.Streams
{
    public class RecordTranslatorRegistry
    {
        private readonly Dictionary<Type, Type> _registeredTranslators = new Dictionary<Type, Type>();
        private readonly IRecordTranslatorFactory _recordTranslatorFactory;

        public RecordTranslatorRegistry(IRecordTranslatorFactory recordTranslatorFactory)
        {
            _recordTranslatorFactory = recordTranslatorFactory;
        }

        public void Add(Type recordToType, Type recordTranslator)
        {
            _registeredTranslators.Add(recordToType, recordTranslator);
        }
        
        
        public IRecordTranslator<TIn, TOut> Get<TIn, TOut>() where TOut : IRequest, new()
        {
            if (_registeredTranslators.ContainsKey(typeof(TOut)))
            {
                var translator = _registeredTranslators[typeof(TOut)];
                return (IRecordTranslator<TIn, TOut>)_recordTranslatorFactory.Create<TIn, TOut>(translator);
            }
            else
            {
                return (IRecordTranslator<TIn, TOut>)null;
            }
        }

        public void Register<TIn, TOut, TRecordTranslator>() 
            where TRecordTranslator : IRecordTranslator<TIn, TOut>
            where TOut : IRequest, new()
        {
            if(_registeredTranslators.ContainsKey(typeof(TIn)))
                throw new AggregateException($"The translator map already includes a type of {typeof(TOut).Name} Only one translator allowed per type");
        }
    }
}