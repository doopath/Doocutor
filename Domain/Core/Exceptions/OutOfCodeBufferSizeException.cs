using System;
namespace Domain.Core.Exceptions
{
    public class OutOfCodeBufferSizeException : Exception
    {
        public OutOfCodeBufferSizeException(string message) : base(message) {}
    }
}
