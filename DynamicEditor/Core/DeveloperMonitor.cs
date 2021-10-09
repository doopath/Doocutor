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
        private const int Padding = 1;
        private readonly ConsoleColor _foreground = ConsoleColor.White;
        private readonly ConsoleColor _background = ConsoleColor.Magenta;

        public DeveloperMonitor(int topOffset, int positionFromTop, int positionFromLeft)
        {
            _topOffset = topOffset;
            _positionFromTop = positionFromTop;
            _positionFromLeft = positionFromLeft;
            _renderTime = 0;
        }

        public void Show()
        {
            Console.CursorTop = Padding;
            Console.ForegroundColor = _foreground;
            Console.BackgroundColor = _background;

            var monitor = GetMonitor();

            // secondLine += new string(' ', firstLine.Length - secondLine.Length - 1);
            // thirdLine += new string(' ', firstLine.Length - thirdLine.Length - 1);

            var output = monitor;//.Split("\n").ToList();

            foreach (var l in output)
            {
                Console.CursorLeft = Console.WindowWidth - l.Length - 2;

                Console.Write(l);
                Console.SetCursorPosition(Console.WindowWidth - Padding, Console.CursorTop + Padding);
            }

            Console.ResetColor();
        }

        public void Update(int topOffset, int positionFromTop, int positionFromLeft, ulong renderTime)
        {
            _topOffset = topOffset;
            _positionFromTop = positionFromTop;
            _positionFromLeft = positionFromLeft;
            _renderTime = renderTime;
        }

        private List<string> GetMonitor()
        {
            var monitor = new List<string>
            {
                $" Top: [ offset: {_topOffset}; pos: {_positionFromTop} ]",
                $" Left: [ pos: {_positionFromLeft} ]",
                $" Render time: {_renderTime}ms "
            };

            var longesLine = monitor.OrderByDescending(l => l.Length).ToArray()[0];

            monitor = monitor.Select(l => l + new string(' ', longesLine.Length - l.Length + 1)).ToList();

            return monitor;
        }
    }
}