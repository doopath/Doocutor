using System;

namespace Domain.Core.Exceptions.NotExitExceptions
{
    public class CommandRecognizingException : NotExitException
    {
        public CommandRecognizingException(string message) : base(message) {}
    }
}
