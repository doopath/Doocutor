using System;

namespace Domain.Core.Exceptions
{
    public class ValueOutOfRangeException : Exception
    {
        public ValueOutOfRangeException(string message) : base(message) { }
    }
}