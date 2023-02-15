namespace Utils.Exceptions.NotExitExceptions
{
    public class UnsupportedCommandException : NotExitException
    {
        public UnsupportedCommandException(string message) : base(message) { }
    }
}
