using System.Linq;
using System.Collections.Generic;

using Doocutor.Core.CodeBuffers.CodePointers;

namespace Doocutor.Core.CodeBuffers
{
    class SourceCodeBuffer : ICodeBuffer
    {
        private static readonly List<string> _sourceCode = InitialSourceCode.GetInitialSourceCode();
        private static readonly int _pointerPosition = InitialSourceCode.GetInitialPointerPosition();

        public int BufferSize { get => _sourceCode.Count; }

        public string GetCode() => string.Join("", _sourceCode.Select((l, i) => $"{i} |  {l}").ToArray());

        public void RemoveCodeBlock(ICodeBlockPointer pointer)
        {
            for (var i = 0; i < pointer.StartLineNumber - pointer.EndLineNumber + 1; i++)
            {
                _sourceCode.RemoveAt(pointer.StartLineNumber);
            }
        }

        public void RemoveLine(int lineNumber)
        {
            throw new System.NotImplementedException();
        }

        public void ReplaceLineAt(int lineNumber, string newLine)
        {
            throw new System.NotImplementedException();
        }

        public void Write(string line)
        {
            throw new System.NotImplementedException();
        }

        public void WriteAfter(int lineNumber, string line)
        {
            throw new System.NotImplementedException();
        }
    }

    internal class InitialSourceCode
    {
        public static List<string> GetInitialSourceCode() => new(new string[] {
            "namespace Doocutor\n",
            "{\n",
            "   internal class Program\n",
            "   {\n",
            "       public static void Main(string[] args)\n",
            "       {\n",
            "       }\n",
            "   }\n",
            "}\n"
        });

        public static int GetInitialPointerPosition() => 6;
    }
}
