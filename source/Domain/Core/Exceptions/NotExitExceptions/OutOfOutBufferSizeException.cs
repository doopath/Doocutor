using System;

namespace Domain.Core.Exceptions.NotExitExceptions;

public class OutOfOutBufferSizeException : NotExitException
{
    public OutOfOutBufferSizeException(string message) : base(message) { }
}
