using System.Collections.Generic;

namespace Domain.Core.CodeBufferContents
{
    public class InitialSourceCodeBufferContent : ICodeBufferContent
    {
        public int CursorPositionFromTop => 6;

        public int CursorPositionFromLeft => 14;

        public List<string> SourceCode => new (new[] {
            "namespace Doocutor",
            "{",
            "    public class Program",
            "    {",
            "        public static void Main(string[] args)",
            "        {",
            "        }",
            "    }",
            "}",
        });
    }
}
