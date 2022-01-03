using System;

namespace Domain.Core.Exceptions.NotExitExceptions
{
    public class InterruptedExecutionException : Exception
    {
        public InterruptedExecutionException(string message) : base(message) {}
    }
}
