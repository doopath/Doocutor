using System;

namespace Doocutor.Core.Exceptions
{
    public class CompiledCodeExecutionException : Exception
    {
        public CompiledCodeExecutionException(string message) : base(message) {}
    }
}