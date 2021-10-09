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
        private readonly int WindowWidth;
        private readonly int WindowHeight;
        private int RightEdge => _outBuffer.Width - 3;
        private int BottomEdge => _outBuffer.Height - 2;
        private const int TopEdge = 1;
        public int TopOffset { get; set; }

        public CuiRender(ICodeBuffer codeBuffer, IOutBuffer outBuffer)
        {
            _codeBuffer = codeBuffer;
            _outBuffer = outBuffer;

            WindowWidth = _outBuffer.Width;
            WindowHeight = _outBuffer.Height;

            TopOffset = 0;

            _developerMonitor = new DeveloperMonitor(
                TopOffset,
                _codeBuffer.CursorPositionFromTop,
                _codeBuffer.CursorPositionFromLeft);
        }

        public void Render()
        {
            FixWindowSize();
            DisableCursor();
            _codeBuffer.AdaptCodeForBufferSize(RightEdge);
            _outBuffer.SetCursorPosition(0, 0);

            var width = _outBuffer.Width;
            var height = _outBuffer.Height;
            var code = _codeBuffer.CodeWithLineNumbers;
            var output = PrepareOutput(width, height, code);
            var renderBottomEdge = height - 1;

            for (var i = 0; i < renderBottomEdge; i++)
                _outBuffer.Write(output[i]);

            FixCursorPosition();
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

        private void FixWindowSize()
        {
            if (_outBuffer.Width != WindowWidth || _outBuffer.Height != WindowHeight)
            {
                _outBuffer.Width = WindowWidth;
                _outBuffer.Height = WindowHeight;
                Render();
            }
        }

        private void FixCursorPosition()
        {
            FixVerticalCursorPosition();
            FixHorizontalCursorPosition();
        }

        private void FixVerticalCursorPosition()
        {
            var internalCursorPositionFromTop = _codeBuffer.CursorPositionFromTop - TopOffset;
            var isItNotFirstLine = TopOffset != 0;

            if (internalCursorPositionFromTop >= BottomEdge)
            {
                TopOffset++;
                Render();
            }
            else if (internalCursorPositionFromTop < TopEdge && isItNotFirstLine)
            {
                TopOffset--;
                Render();
            }
        }

        private void FixHorizontalCursorPosition()
        {
            if (_codeBuffer.CursorPositionFromLeft > RightEdge)
            {
                if (_codeBuffer.CursorPositionFromTop >= _codeBuffer.BufferSize)
                    _codeBuffer.IncreaseBufferSize();

                _codeBuffer.SetCursorPositionFromLeftAt(_codeBuffer.GetPrefixLength());
                _codeBuffer.IncCursorPositionFromTop();
                Render();
            }
        }

        private void UpdateCursorPosition()
            => _outBuffer.SetCursorPosition(
                _codeBuffer.CursorPositionFromLeft,
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
                _codeBuffer.CursorPositionFromTop,
                _codeBuffer.CursorPositionFromLeft);

        private List<string> PrepareOutput(int width, int height, string code)
        {
            var output = GetOutput(width, code);

            if (output.Count < height)
            {
                var emptyLinesCount = height - output.Count;

                for (var i = 0; i < emptyLinesCount; i++)
                    output.Add(new string(' ', width));
            }

            return output;
        }

        private List<string> GetOutput(int width, string code)
            => code
                .Split("\n")[TopOffset..]
                .AsParallel()
                .Select(l => l + (l.Length < width ? new string(' ', width - l.Length) : ""))
                .ToList();
    }
}
