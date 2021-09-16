using System;
using Domain.Core.CodeBuffers;

namespace DynamicEditor.Core
{
    internal sealed class CuiRender
    {
        private readonly ICodeBuffer _codeBuffer;
        public int TopOffset { get; set; }
        public int LeftOffset { get; set; }

        public CuiRender(ICodeBuffer codeBuffer)
        {
            _codeBuffer = codeBuffer;
            TopOffset = 0;
        }

        public void Render()
        {
            var code = _codeBuffer.CodeWithLineNumbers;
            var codeLines = code.Split("\n")[TopOffset..];
            var width = Console.WindowWidth;
            var height = Console.WindowHeight;

            DisableCursor();
            Console.SetCursorPosition(0, 0);

            for (var t = 0; t < height - 1; t++)
            {
                var line = codeLines.Length < t + 1 ? new string(' ', width) : codeLines[t];
                line += line.Length < width ? new string(' ', width - line.Length) : "";

                Console.Write(line);
            }
            
            FixCursorPosition(width, height);
            UpdateCursorPosition();
            EnableCursor();
        }

        public void Clear()
            => Console.Clear();

        public void MoveCursorUp()
        {

            _codeBuffer.DecCursorPositionFromTop();
            Render();
        }

        public void MoveCursorDown()
        {
            _codeBuffer.IncCursorPositionFromTop();
            Render();
        }

        public void MoveCursorLeft()
        {
            _codeBuffer.DecCursorPositionFromLeft();
            Render();
        }
        
        public void MoveCursorRight()
        {
            _codeBuffer.IncCursorPositionFromLeft();
            Render();
        }
        
        private void EnableCursor()
            => Console.CursorVisible = true;

        private void DisableCursor()
            => Console.CursorVisible = false;
        
        private void FixCursorPosition(int width, int height)
        {
            if (_codeBuffer.CursorPositionFromTop >= height)
            {
                _codeBuffer.SetCursorPositionFromTopAt(_codeBuffer.CursorPositionFromTop - 1);
                TopOffset++;
            }
        }

        private void UpdateCursorPosition()
            => Console.SetCursorPosition(_codeBuffer.CursorPositionFromLeft, _codeBuffer.CursorPositionFromTop);
    }
}
