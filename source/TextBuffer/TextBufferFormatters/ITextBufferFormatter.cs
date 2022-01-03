namespace TextBuffer.TextBufferFormatters
{
    public interface ITextBufferFormatter
    {
        List<string> SourceText { get; }
        int? MaxLineLength { get; }
        string GroupNewLineOfACurrentOne(string newPart,
                int cursorPositionFromTop, int cursorPositionFromLeft);
        string GroupOutputLineAt(int lineNumber);
        string GetSourceCodeWithLineNumbers();
        string SeparateLineFromLineNumber(string line);
        bool AdaptCodeForBufferSize(int maxLineLength);
        string GetLineAt(int lineNumber);
        string GetSourceCode();
        int IndexToLineNumber(int index);
        int LineNumberToIndex(int lineNumber);
        int GetPrefixLength(int currentLineNumber);
    }
}
