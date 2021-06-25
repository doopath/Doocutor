using System;

namespace Doocutor.Core.Exceptions
{
    public class CacheOverflowException : Exception
    {
        public CacheOverflowException(string message) : base(message) {}
    }
}