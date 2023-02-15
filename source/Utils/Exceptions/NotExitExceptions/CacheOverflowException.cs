namespace Utils.Exceptions.NotExitExceptions
{
    public class CacheOverflowException : NotExitException
    {
        public CacheOverflowException(string message) : base(message) { }
    }
}