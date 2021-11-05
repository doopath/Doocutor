using System;
using System.Collections.Generic;
using System.Diagnostics;
using Domain.Core.CodeBuffers;
using Domain.Core.ColorSchemes;
using Domain.Core.OutBuffers;
using Domain.Core.Scenes;
using Pastel;

namespace DynamicEditor.Core
{
    /// <summary>
    /// Render some text to the out buffer.
    /// Usually the out buffer is the stdin.
    /// It doesn't display real cursor of the out buffer
    /// (for being more faster), it uses the virtual one
    /// (a colorized symbol).
    /// </summary>
    public sealed class CuiRender
    {
        /// <summary>
        /// Offset value from the top of the code buffer.
        /// Is used to scrolling rendered content.
        /// </summary>
        public int TopOffset { get; set; }
        
        /// <summary>
        /// Enable/disable developer monitor
        /// (a panel that's displayed at the right top corner
        /// of the out buffer and shows render time, position, etc).
        /// Usually disabled in 'release' version.
        /// </summary>
        public bool IsDeveloperMonitorShown { get; set; }
        
        /// <summary>
        /// Set color scheme for the rendered text and the
        /// developer monitor.
        /// </summary>
        public IColorScheme ColorScheme
        {
            get => _colorScheme;
            set
            {
                _colorScheme = value;
                _developerMonitor.ColorScheme = value;
            }
        }

        private static readonly object RenderLocker = new();
        private readonly ICodeBuffer _codeBuffer;
        private readonly DeveloperMonitor _developerMonitor;
        private readonly IOutBuffer _outBuffer;
        private readonly IScene _scene;
        private IColorScheme _colorScheme;
        private List<string> _pureScene;
        private int WindowWidth => _outBuffer.Width;
        private int WindowHeight => _outBuffer.Height;
        private int RightEdge => _outBuffer.Width - 2;
        private int BottomEdge => _outBuffer.Height - 1;
        private Stopwatch _watch;
        private long _lastFrameRenderTime;
        private const int TopEdge = 0;

        /// <param name="codeBuffer">
        /// A code buffer (for example: Domain.Core.CodeBuffers.SourceCodeBuffer).
        /// </param>
        /// <param name="outBuffer">
        /// The out buffer which should be used to render a scene. For example:
        /// Domain.Core.OutBuffers.StandardConsoleOutBuffer.
        /// </param>
        /// <param name="scene">
        /// The scene which will be composed and rendered in the out buffer.
        /// </param>
        /// <param name="colorScheme">
        /// A color scheme which is used to colorize output.
        /// </param>
        public CuiRender(ICodeBuffer codeBuffer, IOutBuffer outBuffer,
                IScene scene, IColorScheme colorScheme)
        {
            _watch = new Stopwatch();
            _codeBuffer = codeBuffer;
            _outBuffer = outBuffer;
            _scene = scene;
            _pureScene = new();

            TopOffset = 0;
            _colorScheme = colorScheme;

            _developerMonitor = new DeveloperMonitor(
                TopOffset,
                _codeBuffer.CursorPositionFromTop,
                _codeBuffer.CursorPositionFromLeft,
                _scene,
                ColorScheme);
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
            SetScene();
            Render(_scene.CurrentScene);
        }

        /// <summary>
        /// Render the scene.
        /// This method uses the monitor, so you can call
        /// it only in single-thread mode.
        /// If the IsDeveloperMonitorShown property equals True,
        /// than the developer monitor also will be rendered.
        /// </summary>
        /// 
        /// <param name="scene">
        /// A list of lines.
        /// </param>
        public void Render(List<string> scene)
        {
            lock (RenderLocker)
            {
                if (IsDeveloperMonitorShown)
                    StartWatching();

                ShowScene(scene);
                RenderCursor();

                if (!IsDeveloperMonitorShown) return;

                StopWatching();
                UpdateDeveloperMonitor();
            }
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

        private void RenderCursor()
        {
            var top = _codeBuffer.CursorPositionFromTop - TopOffset;
            var left = _codeBuffer.CursorPositionFromLeft;
            var initialCursorPosition = (_outBuffer.CursorLeft, _outBuffer.CursorTop);
            var symbol = _pureScene[top][left];

            _outBuffer.SetCursorPosition(left, top);
            _outBuffer.Write(symbol
                .ToString()
                .Pastel(ColorScheme.CursorForeground)
                .PastelBg(ColorScheme.CursorBackground));

            (_outBuffer.CursorLeft, _outBuffer.CursorTop) = initialCursorPosition;
        }

        private void SetScene()
        {
            if (IsDeveloperMonitorShown)
                UpdateDeveloperMonitor();

            _codeBuffer.AdaptCodeForBufferSize(RightEdge);

            _pureScene = _scene.GetNewScene(
                _codeBuffer.CodeWithLineNumbers, WindowWidth, WindowHeight, TopOffset);

            _scene.ComposeOf(_pureScene);
        }

        private void ShowScene(List<string> scene)
        {
            _outBuffer.SetCursorPosition(0, 0);
            _outBuffer.WriteLine(string.Join("", scene));

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
