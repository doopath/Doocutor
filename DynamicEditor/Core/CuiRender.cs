using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.CodeBuffers;
using Domain.Core.OutBuffers;

namespace DynamicEditor.Core
{
    internal sealed class CuiRender
    {
        private readonly ICodeBuffer _codeBuffer;
        private readonly DeveloperMonitor _developerMonitor;
        private readonly IOutBuffer _outBuffer;
        public int TopOffset { get; set; }
        public int LeftOffset { get; set; }

        public CuiRender(ICodeBuffer codeBuffer, IOutBuffer outBuffer)
        {
            _codeBuffer = codeBuffer;
            _outBuffer = outBuffer;

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
            var width = _outBuffer.Width;
            var height = _outBuffer.Height;
            var output = PrepareOutput(width, height, code);

            DisableCursor();
            _outBuffer.SetCursorPosition(0, 0);

            for (var i = 0; i < height - 1; i++)
                _outBuffer.Write(output[i]);

            FixCursorPosition(width, height);
            UpdateCursorPosition();
            ShowDeveloperMonitor(); // Dev-only feature
            EnableCursor();
        }

        public void Clear()
            => _outBuffer.Clear();

        public void MoveCursorUp()
            => DoCursorMovement(_codeBuffer.DecCursorPositionFromTop);

        public void MoveCursorDown()
            => DoCursorMovement(_codeBuffer.IncCursorPositionFromTop);

        public void MoveCursorLeft()
            => DoCursorMovement(_codeBuffer.DecCursorPositionFromLeft);

        public void MoveCursorRight()
            => DoCursorMovement(_codeBuffer.IncCursorPositionFromLeft);

        public void ShowRenderTime()
        {
            var watch = new System.Diagnostics.Stopwatch();

            watch.Start();
            Render();
            watch.Stop();

            _outBuffer.Clear();
            _outBuffer.WriteLine($"Time spent: {watch.ElapsedMilliseconds}ms");
        }

        private void DoCursorMovement(Action movement)
        {
            try
            {
                movement?.Invoke();
            }
            finally
            {
                Render();
            }
        }

        private void EnableCursor()
            => _outBuffer.CursorVisible = true;

        private void DisableCursor()
            => _outBuffer.CursorVisible = false;

        private void FixCursorPosition(int width, int height)
        {
            var bottomEdge = height - 2;
            var topEdge = 1;

            var internalCursorPositionFromTop = _codeBuffer.CursorPositionFromTop - TopOffset;
            var isItNotFirstLine = TopOffset != 0;

            if (internalCursorPositionFromTop >= bottomEdge)
            {
                TopOffset++;
                Render();
            }
            else if (internalCursorPositionFromTop < topEdge && isItNotFirstLine)
            {
                TopOffset--;
                Render();
            }
        }

        private void UpdateCursorPosition()
            => _outBuffer.SetCursorPosition(
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

        private List<string> PrepareOutput(int width, int height, string code)
        {
            var output = code
                .Split("\n")[TopOffset..]
                .AsParallel()
                .Select(l => l + (l.Length < width ? new string(' ', width - l.Length) : ""))
                .ToList();

            if (output.Count < height)
            {
                var emptyLinesCount = height - output.Count;

                for (var i = 0; i < emptyLinesCount; i++)
                    output.Add(new string(' ', width));
            }

            return output;
        }
    }
}
