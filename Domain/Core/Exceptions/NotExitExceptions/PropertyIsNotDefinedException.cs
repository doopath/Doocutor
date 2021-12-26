using System;

namespace Domain.Core.Exceptions.NotExitExceptions
{
    public class PropertyIsNotDefinedException : NotExitException
    {
        public PropertyIsNotDefinedException(string message) : base(message) {}
    }
}