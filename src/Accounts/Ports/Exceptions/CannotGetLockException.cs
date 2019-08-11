using System;

namespace Accounts.Ports.Exceptions
{
    public class CannotGetLockException : Exception
    {
        public CannotGetLockException() {}

        public CannotGetLockException(string message) : base(message){}

        public CannotGetLockException(string message, Exception innerException) : base(message, innerException){}
    }
}