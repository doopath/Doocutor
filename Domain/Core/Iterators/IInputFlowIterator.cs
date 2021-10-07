namespace Domain.Core.Iterators
{
    /// <summary>
    /// A descriptor for InputFlowHandler. It takes if
    /// those exist commands and returns it. You can set a specific descriptor
    /// (that may be a file or stdin).
    /// </summary>
    public interface IInputFlowIterator
    {
        bool HasNext();
        string Next();
    }
}
