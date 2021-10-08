using System;
using System.Linq;

namespace DynamicEditor.Core
{
    internal sealed class DeveloperMonitor
    {
        private int _topOffset;
        private int _positionFromTop;
        private int _positionFromLeft;
        private const int Padding = 1;
        private readonly ConsoleColor _foreground = ConsoleColor.White;
        private readonly ConsoleColor _background = ConsoleColor.Magenta;

        public DeveloperMonitor(int topOffset, int positionFromTop, int positionFromLeft)
        {
            _topOffset = topOffset;
            _positionFromTop = positionFromTop;
            _positionFromLeft = positionFromLeft;
        }

        public void Show()
        {
            Console.CursorTop = Padding;
            Console.ForegroundColor = _foreground;
            Console.BackgroundColor = _background;

            var firstLine = $" Top: [ offset: {_topOffset}; pos: {_positionFromTop} ] \n";
            var secondLine = $" Left: [ pos: {_positionFromLeft} ] ";

            secondLine += new string(' ', firstLine.Length - secondLine.Length - 1);

            var monitor = firstLine + secondLine;
            var output = monitor.Split("\n").ToList();

            foreach (var l in output)
            {
                Console.CursorLeft = Console.WindowWidth - l.Length - 2;

                Console.Write(l);
                Console.SetCursorPosition(Console.WindowWidth - Padding, Console.CursorTop + Padding);
            }

            Console.ResetColor();
        }

        public void Update(int topOffset, int positionFromTop, int positionFromLeft)
        {
            _topOffset = topOffset;
            _positionFromTop = positionFromTop;
            _positionFromLeft = positionFromLeft;
        }
    }
}