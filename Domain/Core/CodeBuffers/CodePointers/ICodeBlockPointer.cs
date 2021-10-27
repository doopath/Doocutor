namespace Domain.Core.CodeBuffers.CodePointers
{
    public interface ICodeBlockPointer
    {
        int StartLineNumber { get; }
        int EndLineNumber { get; }
    }
}
