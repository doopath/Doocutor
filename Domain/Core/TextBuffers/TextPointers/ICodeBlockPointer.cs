namespace Domain.Core.TextBuffers.TextPointers
{
    public interface ITextBlockPointer
    {
        int StartLineNumber { get; }
        int EndLineNumber { get; }
    }
}
