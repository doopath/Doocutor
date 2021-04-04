using System;

namespace Doocutor.Core.Exceptions
{
    internal class CommandExecutingException : Exception
    {
        public CommandExecutingException(string message) : base(message) {}
    }
}
