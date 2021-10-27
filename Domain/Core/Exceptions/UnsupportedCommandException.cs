using System;

namespace Domain.Core.Exceptions
{
    public class UnsupportedCommandException : Exception
    {
        public UnsupportedCommandException(string message) : base(message) {}
    }
}
