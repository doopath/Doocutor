using System;

namespace Doocutor.Core.Exceptions
{
    internal class UnsupportedCommandException : Exception
    {
        public UnsupportedCommandException(string message) : base(message) {}
    }
}
