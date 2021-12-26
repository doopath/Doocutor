using System;

namespace Domain.Core.Exceptions.NotExitExceptions
{
    public class CommandExecutingException : NotExitException
    {
        public CommandExecutingException(string message) : base(message) {}
    }
}
