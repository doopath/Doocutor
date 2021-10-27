using System;

namespace Domain.Core.Exceptions
{
    public class UnsupportedOSPlatformException : Exception
    {
        public UnsupportedOSPlatformException(string message) : base(message) {}
    }
}