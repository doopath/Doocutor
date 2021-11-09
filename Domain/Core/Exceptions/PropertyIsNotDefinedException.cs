using System;

namespace Domain.Core.Exceptions
{
    public class PropertyIsNotDefinedException : Exception
    {
        public PropertyIsNotDefinedException(string message) : base(message) {}
    }
}