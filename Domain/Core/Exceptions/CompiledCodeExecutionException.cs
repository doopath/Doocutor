using System;

namespace Domain.Core.Exceptions
{
    public class CompiledCodeExecutionException : Exception
    {
        public CompiledCodeExecutionException(string message) : base(message) {}
    }
}