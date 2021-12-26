using System;

namespace Domain.Core.Exceptions.NotExitExceptions
{
    public class UnsupportedOSPlatformException : Exception
    {
        public UnsupportedOSPlatformException(string message) : base(message) {}
    }
}