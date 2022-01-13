namespace TextBuffer.TextBufferContents;

public sealed record TextBufferContent : ITextBufferContent
{
    public List<string> SourceCode { get; init; } = new();
    public int CursorPositionFromTop { get; init; }
    public int CursorPositionFromLeft { get; init; }
}