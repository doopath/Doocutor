using System;

namespace Domain.Core.Exceptions
{
    public class IncorrectRangeException : Exception
    {
        public IncorrectRangeException(string message) : base(message) {}
    }
}