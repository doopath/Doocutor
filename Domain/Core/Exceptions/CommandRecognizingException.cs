using System;

namespace Domain.Core.Exceptions
{
    public class CommandRecognizingException : Exception
    {
        public CommandRecognizingException(string message) : base(message) {}
    }
}
