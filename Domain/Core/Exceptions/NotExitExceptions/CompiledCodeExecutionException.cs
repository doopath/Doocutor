using System;

namespace Domain.Core.Exceptions.NotExitExceptions
{
    public class CompiledCodeExecutionException : NotExitException
    {
        public CompiledCodeExecutionException(string message) : base(message) {}
    }
}