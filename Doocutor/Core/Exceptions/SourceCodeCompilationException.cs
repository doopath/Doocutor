using System;

namespace Doocutor.Core.Exceptions
{
    internal class SourceCodeCompilationException : Exception
    {
        public SourceCodeCompilationException(string message) : base(message) {}
    }
}