using System;

namespace Domain.Core.Exceptions.NotExitExceptions
{
    public class IncorrectRangeException : NotExitException
    {
        public IncorrectRangeException(string message) : base(message) {}
    }
}