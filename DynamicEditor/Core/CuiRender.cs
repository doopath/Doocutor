using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Domain.Core.CodeBuffers;
using Domain.Core.OutBuffers;
using Domain.Core.Scenes;
using Spectre.Console;

namespace DynamicEditor.Core
{
    internal sealed class CuiRender
    {
        private readonly ICodeBuffer _codeBuffer;
        private readonly DeveloperMonitor _developerMonitor;
        private readonly IOutBuffer _outBuffer;
        private readonly IScene _scene;
        private int WindowWidth => _outBuffer.Width;
        private int WindowHeight => _outBuffer.Height;
        private int RightEdge => _outBuffer.Width - 3;
        private int BottomEdge => _outBuffer.Height - 2;
        private Stopwatch _watch;
        private long _lastFrameRenderTime;
        private const int TopEdge = 1;
        public int TopOffset { get; set; }

        public CuiRender(ICodeBuffer codeBuffer, IOutBuffer outBuffer, IScene scene)
        {
            _watch = new Stopwatch();
            _codeBuffer = codeBuffer;
            _outBuffer = outBuffer;
            _scene = scene;

            TopOffset = 0;

            _developerMonitor = new DeveloperMonitor(
                TopOffset,
                _codeBuffer.CursorPositionFromTop,
                _codeBuffer.CursorPositionFromLeft);
        }

        public void Render()
        {
            lock (this)
                ShowFrame(GetScene());
        }

        public void Render(List<string> scene)
        {
            lock (this)
                ShowFrame(scene);
        }

        public List<string> GetScene()
        {
            _codeBuffer.AdaptCodeForBufferSize(RightEdge);
            _scene.Compose(_codeBuffer.CodeWithLineNumbers, WindowWidth, WindowHeight, TopOffset);

            return _scene.CurrentScene;
        }

        private void ShowFrame(List<string> scene)
        {
            StartWatching(); // Disable this if DeveloperMonitor is disabled;
            DisableCursor();

            _outBuffer.SetCursorPosition(0, 0);
            _outBuffer.Fill(scene);
            
            FixCursorPosition();
            UpdateCursorPosition();

            StopWatching(); // Disable this if DeveloperMonitor is disabled;
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

        private void StartWatching()
            => _watch.Start();

        private void StopWatching()
        {
            _watch.Stop();
            _lastFrameRenderTime = _watch.ElapsedMilliseconds;
            _watch = new Stopwatch();
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

                var targetPositionFromLeft = _codeBuffer.GetPrefixLength() + 1;

                _codeBuffer.SetCursorPositionFromLeftAt(targetPositionFromLeft);
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
               _codeBuffer.CursorPositionFromLeft,
               (ulong)_lastFrameRenderTime);
    }
}
