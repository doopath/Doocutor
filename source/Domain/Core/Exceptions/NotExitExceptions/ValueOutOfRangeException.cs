using System;

namespace Domain.Core.Exceptions.NotExitExceptions
{
    public class ValueOutOfRangeException : NotExitException
    {
        public ValueOutOfRangeException(string message) : base(message) { }
    }
}