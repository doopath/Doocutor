using Domain.Core.CodeBuffers.CodePointers;

namespace Domain.Core.CodeBuffers
{
    public interface ICodeBuffer
    {
        void Write(string line);
        void WriteAfter(int lineNumber, string line);
        void WriteBefore(int lineNumber, string line);
        void AppendLine(string newPart);
        void Enter();
        void Backspace();
        void RemoveLineAt(int lineNumber);
        void IncreaseBufferSize();
        int GetPrefixLength();
        void RemoveCodeBlock(ICodeBlockPointer pointer);
        void ReplaceLineAt(int lineNumber, string newLine);
        void AdaptCodeForBufferSize(int maxLineLength);
        void SetCursorPositionFromTopAt(int lineNumber);
        void SetCursorPositionFromLeftAt(int position);
        void IncCursorPositionFromLeft();
        void DecCursorPositionFromLeft();
        void IncCursorPositionFromTop();
        void DecCursorPositionFromTop();
        string GetLineAt(int lineNumber);
        string[] GetCodeBlock(ICodeBlockPointer pointer);
        string CodeWithLineNumbers { get; }
        string Code { get; }
        string CurrentLine { get; }
        string[] Lines { get; }
        int Size { get; }
        int CursorPositionFromLeft { get; }
        int CursorPositionFromTop { get; }
    }
}
