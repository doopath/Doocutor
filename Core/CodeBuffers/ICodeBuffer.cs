﻿using Doocutor.Core.CodeBuffers.CodePointers;

namespace Doocutor.Core.CodeBuffers
{
    interface ICodeBuffer
    {
        void Write(string line);
        void WriteAfter(int lineNumber, string line);
        void RemoveLine(int lineNumber);
        void RemoveCodeBlock(ICodeBlockPointer pointer);
        void ReplaceLineAt(int lineNumber, string newLine);
        string GetCode();
        int BufferSize { get; }
    }
}