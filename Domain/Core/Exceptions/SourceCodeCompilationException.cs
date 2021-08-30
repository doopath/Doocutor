using System;

namespace Domain.Core.Exceptions
{
    public class SourceCodeCompilationException : Exception
    {
        public SourceCodeCompilationException(string message) : base(message) {}
    }
}