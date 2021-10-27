using System;

namespace Domain.Core.Exceptions
{
    public class CodeBlockPointerValidationException : Exception
    {
        public CodeBlockPointerValidationException(string message) : base(message) { }
    }
}
