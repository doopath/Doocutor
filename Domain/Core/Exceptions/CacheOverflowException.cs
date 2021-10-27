using System;

namespace Domain.Core.Exceptions
{
    public class CacheOverflowException : Exception
    {
        public CacheOverflowException(string message) : base(message) {}
    }
}