using System;

namespace Domain.Core.Exceptions.NotExitExceptions
{
    public class CodeBlockPointerValidationException : NotExitException
    {
        public CodeBlockPointerValidationException(string message) : base(message) { }
    }
}
