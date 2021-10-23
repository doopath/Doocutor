using System;
using System.Collections.Generic;
using System.Diagnostics;
using Domain.Core.CodeBuffers;
using Domain.Core.OutBuffers;
using Domain.Core.Scenes;
using Pastel;

namespace DynamicEditor.Core
{
    public sealed class CuiRender
    {
        private const string CursorForeground = "#000000";
        private const string CursorBackground = "#ffffff";
        private readonly ICodeBuffer _codeBuffer;
        private readonly DeveloperMonitor _developerMonitor;
        private readonly IOutBuffer _outBuffer;
        private readonly IScene _scene;
        private int WindowWidth => _outBuffer.Width;
        private int WindowHeight => _outBuffer.Height;
        private int RightEdge => _outBuffer.Width - 2;
        private int BottomEdge => _outBuffer.Height - 1;
        private Stopwatch _watch;
        private long _lastFrameRenderTime;
        private const int TopEdge = 0;
        public int TopOffset { get; set; }
        public bool IsDeveloperMonitorShown { get; set; }

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
                _codeBuffer.CursorPositionFromLeft,
                _scene);
        }

        public void EnableDeveloperMonitor()
        {
            IsDeveloperMonitorShown = true;
            _developerMonitor.TurnOn();
        }

        public void DisableDeveloperMonitor()
        {
            IsDeveloperMonitorShown = false;
            _developerMonitor.TurnOff();
        }

        public void Render()
            => Render(GetScene());

        public void Render(List<string> scene)
        {
            lock (this)
            {
                if (IsDeveloperMonitorShown)
                    StartWatching();

                CleanPaddingArea();
                ShowFrame(scene);
                RenderCursor();

                if (!IsDeveloperMonitorShown) return;
                
                StopWatching(); // Disable this if DeveloperMonitor is disabled;
                UpdateDeveloperMonitor();
            }
        }
        
        private void RenderCursor()
        {
            var top = _codeBuffer.CursorPositionFromTop - TopOffset;
            var left = _codeBuffer.CursorPositionFromLeft;
            var initialCursorPosition = (_outBuffer.CursorLeft, _outBuffer.CursorTop);
            var newScene = _scene.GetNewScene(
                _codeBuffer.CodeWithLineNumbers, _outBuffer.Width, _outBuffer.Height, TopOffset);
            var symbol = newScene[top][left];
            
            _outBuffer.SetCursorPosition(left, top);
            _outBuffer.Write(symbol.ToString().Pastel(CursorForeground).PastelBg(CursorBackground));
            
            (_outBuffer.CursorLeft, _outBuffer.CursorTop) = initialCursorPosition;
        }

        public List<string> GetScene()
        {
            UpdateDeveloperMonitor(); // dev-only feature; 

            _codeBuffer.AdaptCodeForBufferSize(RightEdge);
            _scene.Compose(_codeBuffer.CodeWithLineNumbers, WindowWidth, WindowHeight, TopOffset);

            return _scene.CurrentScene;
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

        private void ShowFrame(List<string> scene)
        {
            _outBuffer.SetCursorPosition(0, 0);
            _outBuffer.Fill(scene);

            FixCursorPosition();
        }
        
        private void CleanPaddingArea()
        {
            var initialCursorPosition = (_outBuffer.CursorTop, _outBuffer.CursorLeft);

            CleanBottomPaddingArea();
            CleanRightPaddingArea();

            (_outBuffer.CursorTop, _outBuffer.CursorLeft) = initialCursorPosition;
        }

        private void CleanBottomPaddingArea()
        {
            for (var i = BottomEdge + 1; i < _outBuffer.Height - 1; i++)
            {
                var emptyLine = new string(' ', _outBuffer.Width);
                _outBuffer.SetCursorPosition(0, i);
                _outBuffer.Write(emptyLine);
            }
        }

        private void CleanRightPaddingArea()
        {
            for (var l = RightEdge; l < _outBuffer.Width; l++)
            {
                for (var t = 0; t <= _outBuffer.Height - 2; t++)
                {
                    var emptyLine = new string(' ', _outBuffer.Width - RightEdge);
                    _outBuffer.SetCursorPosition(l, t);
                    _outBuffer.Write(emptyLine);
                }
            }
        }

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
                if (_codeBuffer.CursorPositionFromTop >= _codeBuffer.Size)
                    _codeBuffer.IncreaseBufferSize();

                var targetPositionFromLeft = _codeBuffer.GetPrefixLength() + 1;

                _codeBuffer.SetCursorPositionFromLeftAt(targetPositionFromLeft);
                _codeBuffer.IncCursorPositionFromTop();
                Render();
            }
        }
        

        private void UpdateDeveloperMonitor()
            => _developerMonitor.Update(
                   TopOffset,
                   _codeBuffer.CursorPositionFromTop,
                   _codeBuffer.CursorPositionFromLeft,
                   (ulong)_lastFrameRenderTime);
    }
}
