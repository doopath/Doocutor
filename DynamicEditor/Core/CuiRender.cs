using System;
using System.Linq;
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
            var width = Console.WindowWidth;
            var height = Console.WindowHeight;
            var codeLines = code.Split("\n")[TopOffset..];

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
            ShowDevMonitor(); // Dev-only feature
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
            var bottomEdge = height - 3;
            var topEdge = 2;

            var internalCursorPositionFromTop = _codeBuffer.CursorPositionFromTop - TopOffset;
            var isItNotFirstLine = TopOffset != 0;

            if (internalCursorPositionFromTop >= bottomEdge)
                TopOffset++;
            else if (internalCursorPositionFromTop < topEdge && isItNotFirstLine)
                TopOffset--;
        }

        private void UpdateCursorPosition()
            => Console.SetCursorPosition(
            _codeBuffer.CursorPositionFromLeft - LeftOffset,
            _codeBuffer.CursorPositionFromTop - TopOffset);

        private void ShowDevMonitor()
        {
            DisableCursor();
            Console.CursorTop = 1;
            Console.ForegroundColor = ConsoleColor.Cyan;

            var monitor = $"Top: [ offset: {TopOffset}; pos: {_codeBuffer.CursorPositionFromTop} ]\n";
            monitor += $"Left: [ offset: {LeftOffset}; pos: {_codeBuffer.CursorPositionFromLeft} ]";

            var output = monitor.Split("\n").ToList();

            foreach (var l in output)
            {
                Console.CursorLeft = Console.WindowWidth - l.Length - 2;

                Console.Write(l);
                Console.SetCursorPosition(Console.WindowWidth - 1, Console.CursorTop + 1);
            }

            Console.ResetColor();
            UpdateCursorPosition();
            EnableCursor();
        }
    }
}
