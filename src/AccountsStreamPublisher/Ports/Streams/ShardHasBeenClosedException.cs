using System;

namespace AccountsTransferWorker.Ports.Streams
{
    public class ShardHasBeenClosedException : Exception
    {
        public ShardHasBeenClosedException() {}

        public ShardHasBeenClosedException(string message) : base(message) {}

        public ShardHasBeenClosedException(string message, Exception baseException) : base(message, baseException) {}
    }
}