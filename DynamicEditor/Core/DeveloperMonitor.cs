using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core;
using Domain.Core.ColorSchemes;
using Domain.Core.Scenes;
using Pastel;

namespace DynamicEditor.Core
{
    public sealed class DeveloperMonitor
    {
        public static IColorScheme? ColorScheme { get; set; } = Settings.ColorScheme;
        private IEnumerable<string> _monitor;
        private readonly IScene _scene;
        private int _topOffset;
        private int _positionFromTop;
        private int _positionFromLeft;
        private ulong _renderTime;
        private ulong _renderedFrames;
        private ulong _renderTimeAcc;
        private int _avgRenderTime;
        private string? _longestMonitorLine;
        private const int Padding = 1;
        private const int AvgFramesCount = 5;

        public DeveloperMonitor(int topOffset, int positionFromTop,
            int positionFromLeft, IScene scene, IColorScheme colorScheme)
        {
            ColorScheme = colorScheme;
            _monitor = new List<string>();
            _scene = scene;
            _topOffset = topOffset;
            _positionFromTop = positionFromTop;
            _positionFromLeft = positionFromLeft;
            _renderTime = 0;
            _avgRenderTime = 0;
            _renderedFrames = 0;
            _renderTimeAcc = 0;
        }

        public void TurnOn()
        {
            _scene.SceneUpdated += OnSceneUpdated(AddMonitor);
            _scene.SceneUpdated += OnSceneUpdated(ColorizeMonitor);
        }

        public void TurnOff()
        {
            _scene.SceneUpdated -= OnSceneUpdated(AddMonitor);
            _scene.SceneUpdated -= OnSceneUpdated(ColorizeMonitor);
        }

        public void Update(int topOffset, int positionFromTop, int positionFromLeft, ulong renderTime)
        {
            _topOffset = topOffset;
            _positionFromTop = positionFromTop;
            _positionFromLeft = positionFromLeft;

            _renderTime = renderTime;
            _renderTimeAcc += renderTime;
            _renderedFrames++;

            if (_renderedFrames == AvgFramesCount)
            {
                _avgRenderTime = (int)_renderTimeAcc / AvgFramesCount;
                _renderTimeAcc = 0;
                _renderedFrames = 0;
            }
        }

        private void AddMonitor(List<string> sceneContent)
        {
            SetMonitor();
            var index = Padding;

            foreach (var monitorLine in _monitor)
            {
                var sceneLine = sceneContent[index];
                var right = sceneLine.Length - monitorLine.Length;

                sceneContent[index] = sceneLine[..right] + monitorLine;
                index++;
            }
        }

        private void SetMonitor()
        {
            var monitor = new List<string>
            {
                $" Top: [ offset: {_topOffset}; pos: {_positionFromTop} ]",
                $" Left: [ pos: {_positionFromLeft} ]",
                $" Render [ avg: {_avgRenderTime}ms; last: {_renderTime}ms ]"
            };

            _longestMonitorLine = monitor
               .OrderByDescending(l => l.Length)
               .ToArray()[0];

            string GetSpacesForShorterLine(string longestLine, string line)
                => new(' ', longestLine.Length - line.Length + 1);

            string GroupLine(string line)
                => line + GetSpacesForShorterLine(_longestMonitorLine, line);

            _monitor = monitor.Select(GroupLine);
        }

        private void ColorizeMonitor(List<string> sceneContent)
        {
            var start = Padding;
            var end = _monitor.Count() + start;

            for (var i = start; i < end; i++)
            {
                var line = sceneContent[i];
                var right = _longestMonitorLine!.Length + 1;
                var monitorLine = line[^right..];
                var result = line[..^right] + monitorLine
                    .Pastel(ColorScheme!.DeveloperMonitorForeground)
                    .PastelBg(ColorScheme.DeveloperMonitorBackground);

                sceneContent[i] = result;
            }
        }

        private EventHandler<SceneUpdatedEventArgs> OnSceneUpdated(Action<List<string>> action)
            => (object? sender, SceneUpdatedEventArgs eventArgs) => action(eventArgs.SceneContent!);
    }
}
