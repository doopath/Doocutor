using System;

namespace Doocutor.Core.Exceptions
{
    internal class CodeBlockPointerValidationException : Exception
    {
        public CodeBlockPointerValidationException(string message) : base(message) { }
    }
}
