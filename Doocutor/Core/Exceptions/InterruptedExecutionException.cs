using System;

namespace Doocutor.Core.Exceptions
{
    class InterruptedExecutionException : Exception
    {
        public InterruptedExecutionException(string message) : base(message) {}
    }
}
