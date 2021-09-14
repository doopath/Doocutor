using System;
using Domain.Core.CodeBuffers;

namespace DynamicEditor.Core
{
    internal sealed class CUIRender
    {
        private readonly ICodeBuffer _codeBuffer;

        public CUIRender(ICodeBuffer codeBuffer)
        {
            _codeBuffer = codeBuffer;
        }

        public void Render()
        {
            var code = _codeBuffer.CodeWithLineNumbers;
            var codeLines = code.Split("\n");
            var width = Console.WindowWidth;
            var height = Console.WindowHeight;

            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);

            for (var t = 0; t < height; t++)
            {
                var line = "";

                if (codeLines.Length < t + 1)
                    line = new string(' ', width);
                else
                    line = codeLines[t];

                if (line.Length < width)
                    line += new string(' ', width - line.Length);

                Console.Write(line);
            }

            Console.SetCursorPosition(0, _codeBuffer.CurrentPointerPosition); // TODO: add a new position for the cursor
            Console.CursorVisible = true;
        }
    }
}
