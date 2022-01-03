namespace TextBuffer.TextBufferContents;

public interface ITextBufferContent
{
    List<string> SourceCode { get; }
    int CursorPositionFromTop { get; }
    int CursorPositionFromLeft { get; }
}
