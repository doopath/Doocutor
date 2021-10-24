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
        private static object RenderLocker = new();
        private const string CursorForeground = "#000000";
        private const string CursorBackground = "#ffffff";
        private readonly ICodeBuffer _codeBuffer;
        private readonly DeveloperMonitor _developerMonitor;
        private readonly IOutBuffer _outBuffer;
        private readonly IScene _scene;
        private List<string> PureScene;
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
        {
            lock (RenderLocker)
            {
                SetScene(); 
                Render(_scene.CurrentScene);
            }                  
        }

        public void Render(List<string> scene)
        {
            lock (RenderLocker)
            {
                if (IsDeveloperMonitorShown)
                    StartWatching();

                ShowFrame(scene);
                RenderCursor();

                if (!IsDeveloperMonitorShown) return;
                
                StopWatching();
                UpdateDeveloperMonitor();
            }
        }

        private void RenderCursor()
        {
            var top = _codeBuffer.CursorPositionFromTop - TopOffset;
            var left = _codeBuffer.CursorPositionFromLeft;
            var initialCursorPosition = (_outBuffer.CursorLeft, _outBuffer.CursorTop);
            var symbol = PureScene[top][left];
            
            _outBuffer.SetCursorPosition(left, top);
            _outBuffer.Write(symbol.ToString().Pastel(CursorForeground).PastelBg(CursorBackground));
            
            (_outBuffer.CursorLeft, _outBuffer.CursorTop) = initialCursorPosition;
        }

        public void SetScene()
        {
            if (IsDeveloperMonitorShown)
                UpdateDeveloperMonitor();

            _codeBuffer.AdaptCodeForBufferSize(RightEdge);

            var scene = _scene.GetNewScene(
                _codeBuffer.CodeWithLineNumbers, WindowWidth, WindowHeight, TopOffset);

            _scene.ComposeOf(scene);
            PureScene = scene;
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
