using System;

namespace Domain.Core.Exceptions;

public class OutOfOutBufferSizeException : Exception
{
    public OutOfOutBufferSizeException(string message) : base(message) { }
}
