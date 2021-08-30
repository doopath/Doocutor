using Domain.Core.CodeBuffers.CodePointers;

namespace Domain.Core.CodeBuffers
{
    public interface ICodeBuffer
    {
        void Write(string line);
        void WriteAfter(int lineNumber, string line);
        void WriteBefore(int lineNumber, string line);
        void RemoveLineAt(int lineNumber);
        void RemoveCodeBlock(ICodeBlockPointer pointer);
        void ReplaceLineAt(int lineNumber, string newLine);
        void SetPointerPositionAt(int lineNumber);
        string GetLineAt(int lineNumber);
        string[] GetCodeBlock(ICodeBlockPointer pointer);
        string CodeWithLineNumbers { get; }
        string Code { get; }
        string[] Lines { get; }
        int BufferSize { get; }
        int CurrentPointerPosition { get; }
    }
}
