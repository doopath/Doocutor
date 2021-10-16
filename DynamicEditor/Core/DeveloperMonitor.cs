using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicEditor.Core
{
    internal sealed class DeveloperMonitor
    {
        private int _topOffset;
        private int _positionFromTop;
        private int _positionFromLeft;
        private ulong _renderTime;
        private ulong _renderedFrames;
        private int _avgRenderTime;
        private const int Padding = 1;

        public DeveloperMonitor(int topOffset, int positionFromTop, int positionFromLeft)
        {
            _topOffset = topOffset;
            _positionFromTop = positionFromTop;
            _positionFromLeft = positionFromLeft;
            _renderTime = 0;
            _avgRenderTime = 0;
            _renderedFrames = 0;
        }

        public void Show()
        {
            Console.CursorTop = Padding;

            var monitor = GetMonitor();
            var output = monitor;

            foreach (var l in output)
            {
                Console.CursorLeft = Console.WindowWidth - l.Length - 2;

                Console.Write($"\u001b[45;1m{l}\u001b[0m");
                Console.SetCursorPosition(Console.WindowWidth - Padding, Console.CursorTop + Padding);
            }
        }

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

        private List<string> GetMonitor()
        {
            var monitor = new List<string>
            {
                $" Top: [ offset: {_topOffset}; pos: {_positionFromTop} ]",
                $" Left: [ pos: {_positionFromLeft} ]",
                $" Render [ avg: {_avgRenderTime}ms; last: {_renderTime}ms ] "
            };

            var longestLine = monitor.OrderByDescending(l => l.Length).ToArray()[0];

            return monitor.Select(l => l + new string(' ', longestLine.Length - l.Length + 1)).ToList();
        }
    }
}