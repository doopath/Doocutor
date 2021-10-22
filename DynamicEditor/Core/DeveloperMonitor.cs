using System.Collections.Generic;
using System.Linq;
using Spectre.Console;
using Pastel;
using Domain.Core.Scenes;

namespace DynamicEditor.Core
{
    public sealed class DeveloperMonitor
    {
        public string MonitorForeground { get; set; }
        public string MonitorBackground {  get; set; }
        private readonly IScene _scene;
        private int _topOffset;
        private int _positionFromTop;
        private int _positionFromLeft;
        private ulong _renderTime;
        private ulong _renderedFrames;
        private int _avgRenderTime;
        private const int Padding = 1;

        public DeveloperMonitor(int topOffset, int positionFromTop, int positionFromLeft, IScene scene)
        {
            MonitorForeground = "#d8dee9";
            MonitorBackground = "#5e81ac";
            _scene = scene;
            _topOffset = topOffset;
            _positionFromTop = positionFromTop;
            _positionFromLeft = positionFromLeft;
            _renderTime = 0;
            _avgRenderTime = 0;
            _renderedFrames = 0;
        }

        public void TurnOn()
            => _scene.OnSceneUpdated += AddMonitor;

        public void TurnOff()
            => _scene.OnSceneUpdated -= AddMonitor;

        public void Update(int topOffset, int positionFromTop, int positionFromLeft, ulong renderTime)
        {
            _topOffset = topOffset;
            _positionFromTop = positionFromTop;
            _positionFromLeft = positionFromLeft;
            _renderTime = renderTime;
            _renderedFrames++;

            // avg(seq(n)) = (x(n) - avg(seq(n-1))) / n
            // where x is an element of the sequence, avg - a function of
            // an average number and n is a number (index) of an element of the
            // sequence.
            _avgRenderTime += ((int)renderTime - _avgRenderTime) / (int)_renderedFrames;
        }

        private void AddMonitor(List<string> sceneContent)
        {
            var monitor = GetMonitor();

            for (var i = Padding; i < monitor.Count + Padding; i++)
            {
                var sceneLine = sceneContent[i];

                var colorPreifxLength = GetColorPrefixLength();
                var monitorLine = monitor[i - Padding];
                var right = sceneLine.Length - monitorLine.Length - Padding + colorPreifxLength;

                sceneContent[i] = sceneLine[..right] + monitorLine;
            }
        }

        private List<string> GetMonitor()
        {
            var monitor = new List<string>
            {
                $" Top: [ offset: {_topOffset}; pos: {_positionFromTop} ]",
                $" Left: [ pos: {_positionFromLeft} ]",
                $" Render [ avg: {_avgRenderTime}ms; last: {_renderTime}ms ] "
            };

            var longestLine = monitor
                .OrderByDescending(l => l.Length)
                .ToArray()[0];

            return ColorizeMonitor(monitor.Select(l => l + new string(' ', longestLine.Length - l.Length + 1) + "\n")).ToList();
        }

        private IEnumerable<string> ColorizeMonitor(IEnumerable<string> monitor)
            => monitor.Select(l => l.Pastel(MonitorForeground).PastelBg(MonitorBackground));

        private int GetColorPrefixLength()
            => "".Pastel(MonitorForeground).PastelBg(MonitorBackground).Length;
    }
}