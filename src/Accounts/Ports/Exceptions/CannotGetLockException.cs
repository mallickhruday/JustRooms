using System;

namespace Accounts.Ports.Exceptions
{
    /// <summary>
    /// AN exception thrown when we cannot lock a guest account record to update it
    /// </summary>
    public class CannotGetLockException : Exception
    {
        /// <summary>
        /// Construct a lock exception
        /// </summary>
        public CannotGetLockException() {}

        /// <summary>
        /// Constuct a lock exception with a user define message
        /// </summary>
        /// <param name="message">A user defined message</param>
        public CannotGetLockException(string message) : base(message){}

        /// <summary>
        /// Construct a lock exception with a user defined message and inner exception
        /// </summary>
        /// <param name="message">A user defined message</param>
        /// <param name="innerException">An inner execption</param>
        public CannotGetLockException(string message, Exception innerException) : base(message, innerException){}
    }
}