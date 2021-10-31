namespace Domain.Core.CodeBufferHistories
{
    public interface ICodeBufferHistory
    {
        void Add(CodeBufferChange change);
        void Clear();
        bool IsEmpty();
        CodeBufferChange Undo();
        CodeBufferChange Redo();
    }
}
