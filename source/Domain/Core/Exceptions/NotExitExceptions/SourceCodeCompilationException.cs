using System;

namespace Domain.Core.Exceptions.NotExitExceptions
{
    public class SourceCodeCompilationException : NotExitException
    {
        public SourceCodeCompilationException(string message) : base(message) {}
    }
}