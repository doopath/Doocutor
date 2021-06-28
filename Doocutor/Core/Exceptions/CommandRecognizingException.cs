using System;

namespace Doocutor.Core.Exceptions
{
    class CommandRecognizingException : Exception
    {
        public CommandRecognizingException(string message) : base(message) {}
    }
}
