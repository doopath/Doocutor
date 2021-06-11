using Doocutor.Core.CodeBuffers.CodePointers;

namespace Doocutor.Core.CodeBuffers
{
    interface ICodeBuffer
    {
        void Write(string line);
        void WriteAfter(int lineNumber, string line);
        void WriteBefore(int lineNumber, string line);
        void RemoveLine(int lineNumber);
        void RemoveCodeBlock(ICodeBlockPointer pointer);
        void ReplaceLineAt(int lineNumber, string newLine);
        void SetPointerPosition(int lineNumber);
        string CodeWithLineNumbers { get; }
        string Code { get; }
        int BufferSize { get; }
        int CurrentPointerPosition { get; }
    }
}
