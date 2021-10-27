using System;

namespace Domain.Core.Exceptions
{
    public class InterruptedExecutionException : Exception
    {
        public InterruptedExecutionException(string message) : base(message) {}
    }
}
