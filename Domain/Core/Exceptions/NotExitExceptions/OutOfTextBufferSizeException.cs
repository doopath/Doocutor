using System;
namespace Domain.Core.Exceptions.NotExitExceptions
{
    public class OutOfTextBufferSizeException : NotExitException
    {
        public OutOfTextBufferSizeException(string message) : base(message) {}
    }
}
