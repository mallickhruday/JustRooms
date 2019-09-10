using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Accounts.Ports.Events;
using Paramore.Brighter;

namespace JustRoomsTests
{
    public class FakeCommandProcessor : IAmACommandProcessor
    {
        private List<IRequest> depositedEvents = new List<IRequest>();
        private List<Guid> depositIds = new List<Guid>();
        
        public bool RaiseAccountEvent {get; private set;}
        public bool AllSent { get; set; }

        void IAmACommandProcessor.Post<T>(T request)
        {
            if(request.GetType() == typeof(AccountEvent))
            {
                RaiseAccountEvent = true;
            }
            
        }

        Task IAmACommandProcessor.PostAsync<T>(T request, bool continueOnCapturedContext, CancellationToken cancellationToken)
        {
            if(request.GetType() == typeof(AccountEvent))
            {
                RaiseAccountEvent = true;
            }
            return Task.CompletedTask;
        }

        public Guid DepositPost<T>(T request) where T : class, IRequest
        {
            if (request.GetType() == typeof(AccountEvent))
            {
                depositedEvents.Add(request);
                RaiseAccountEvent = true;
            }

            var depositId = Guid.NewGuid();
            depositIds.Add(depositId);

            return depositId;
        }

        public async Task<Guid> DepositPostAsync<T>(T request, bool continueOnCapturedContext = false,
            CancellationToken cancellationToken = new CancellationToken()) where T : class, IRequest
        {
            throw new NotImplementedException();
        }

        public void ClearOutbox(params Guid[] posts)
        {
            AllSent = true;
            foreach (var post in posts)
            {
                if (!depositIds.Contains(post))
                {
                    AllSent = false;
                    break;
                }
            }
        }


        public async Task ClearOutboxAsync(IEnumerable<Guid> posts, bool continueOnCapturedContext = false,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public TResponse Call<T, TResponse>(T request, int timeOutInMilliseconds) where T : class, ICall where TResponse : class, IResponse
        {
            throw new NotImplementedException();
        }

        void IAmACommandProcessor.Publish<T>(T @event)
        {
            throw new System.NotImplementedException();
        }

        Task IAmACommandProcessor.PublishAsync<T>(T @event, bool continueOnCapturedContext, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        void IAmACommandProcessor.Send<T>(T command)
        {
            throw new System.NotImplementedException();
        }

        Task IAmACommandProcessor.SendAsync<T>(T command, bool continueOnCapturedContext, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}