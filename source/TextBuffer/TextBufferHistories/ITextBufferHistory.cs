namespace TextBuffer.TextBufferHistories;

public interface ITextBufferHistory : IEnumerable<ITextBufferChange>
{
    int Limit { get; set; }
    uint Size { get; }
    void Add(ITextBufferChange change);
    void Clear();
    bool IsEmpty();
    ITextBufferChange Undo();
    ITextBufferChange Redo();
}

