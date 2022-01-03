using System;

namespace Domain.Core.Exceptions.NotExitExceptions
{
    public class CacheOverflowException : NotExitException
    {
        public CacheOverflowException(string message) : base(message) {}
    }
}