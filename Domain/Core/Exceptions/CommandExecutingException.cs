using System;

namespace Domain.Core.Exceptions
{
    public class CommandExecutingException : Exception
    {
        public CommandExecutingException(string message) : base(message) {}
    }
}
