using Domain.Core.TextBuffers.TextPointers;

namespace Domain.Core.TextBuffers;

public interface ITextBuffer
{
    void Write(string line);
    void WriteAfter(int lineNumber, string line);
    void WriteBefore(int lineNumber, string line);
    void PasteText(string text);
    void AppendLine(string newPart);
    void Enter();
    void Backspace();
    void RemoveLineAt(int lineNumber);
    void IncreaseBufferSize();
    int GetPrefixLength();
    void RemoveTextBlock(ITextBlockPointer pointer);
    void ReplaceLineAt(int lineNumber, string newLine);
    void AdaptTextForBufferSize(int maxLineLength);
    void SetCursorPositionFromTopAt(int lineNumber);
    void SetCursorPositionFromLeftAt(int position);
    void IncCursorPositionFromLeft();
    void DecCursorPositionFromLeft();
    void IncCursorPositionFromTop();
    void DecCursorPositionFromTop();
    void Undo();
    void Redo();
    string GetLineAt(int lineNumber);
    string[] GetCodeBlock(ITextBlockPointer pointer);
    uint HistoryLimit { get; set; }
    string CodeWithLineNumbers { get; }
    string Code { get; }
    string CurrentLine { get; }
    string[] Lines { get; }
    int Size { get; }
    int CursorPositionFromLeft { get; }
    int CursorPositionFromTop { get; }
}
