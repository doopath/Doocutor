using System.Collections.Generic;

namespace Domain.Core.CodeFormatters
{
    public interface ICodeFormatter
    {
        List<string> SourceCode { get; }
        string GroupNewLineOfACurrentOne(string newPart, int cursorPositionFromTop, int cursorPositionFromLeft);
        string GroupOutputLineAt(int lineNumber);
        string GetSourceCodeWithLineNumbers();
        string SeparateLineFromLineNumber(string line);
        string GetTabulationForLineAt(int lineNumber, string line);
        void AdaptCodeForBufferSize(int maxLineLength);
        string ModifyLine(string line, int lineNumber);
        string GetLineAt(int lineNumber);
        string GetSourceCode();
        int IndexToLineNumber(int index);
        int LineNumberToIndex(int lineNumber);
        int GetPrefixLength(int currentLineNumber);
    }
}