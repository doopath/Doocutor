using System;
using Domain.Core.CodeBuffers;

namespace DynamicEditor.Core
{
    internal sealed class CuiRender
    {
        private readonly ICodeBuffer _codeBuffer;
        private readonly DeveloperMonitor _developerMonitor;
        public int TopOffset { get; set; }
        public int LeftOffset { get; set; }

        public CuiRender(ICodeBuffer codeBuffer)
        {
            _codeBuffer = codeBuffer;

            TopOffset = 0;
            LeftOffset = 0;

            _developerMonitor = new DeveloperMonitor(
                TopOffset,
                LeftOffset,
                _codeBuffer.CursorPositionFromTop,
                _codeBuffer.CursorPositionFromLeft);
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
            ShowDeveloperMonitor(); // Dev-only feature
            EnableCursor();
        }

        public void Clear()
            => Console.Clear();

        public void MoveCursorUp()
            => DoCursorMovement(_codeBuffer.DecCursorPositionFromTop);

        public void MoveCursorDown()
            => DoCursorMovement(_codeBuffer.IncCursorPositionFromTop);

        public void MoveCursorLeft()
        {
            DoCursorMovement(_codeBuffer.DecCursorPositionFromLeft);
        }

        public void MoveCursorRight()
            => DoCursorMovement(_codeBuffer.IncCursorPositionFromLeft);

        private void DoCursorMovement(Action movement)
        {
            Render();
            movement?.Invoke();
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

        private void ShowDeveloperMonitor()
        {
            DisableCursor();

            UpdateDeveloperMonitor();
            _developerMonitor.Show();

            UpdateCursorPosition();
            EnableCursor();
        }

        private void UpdateDeveloperMonitor()
            => _developerMonitor.Update(
                TopOffset,
                LeftOffset,
                _codeBuffer.CursorPositionFromTop,
                _codeBuffer.CursorPositionFromLeft);
    }
}
