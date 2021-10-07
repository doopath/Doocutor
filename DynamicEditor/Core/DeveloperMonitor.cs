using System;
using System.Linq;

namespace DynamicEditor.Core
{
    internal sealed class DeveloperMonitor
    {
        private int _topOffset;
        private int _leftOffset;
        private int _positionFromTop;
        private int _positionFromLeft;

        public DeveloperMonitor(int topOffset, int leftOffset, int positionFromTop, int positionFromLeft)
        {
            _topOffset = topOffset;
            _leftOffset = leftOffset;
            _positionFromTop = positionFromTop;
            _positionFromLeft = positionFromLeft;
        }

        public void Show()
        {
            Console.CursorTop = 1;
            Console.ForegroundColor = ConsoleColor.Cyan;

            var monitor = $"Top: [ offset: {_topOffset}; pos: {_positionFromTop} ]\n";
            monitor += $"Left: [ offset: {_leftOffset}; pos: {_positionFromLeft} ]";

            var output = monitor.Split("\n").ToList();

            foreach (var l in output)
            {
                Console.CursorLeft = Console.WindowWidth - l.Length - 2;

                Console.Write(l);
                Console.SetCursorPosition(Console.WindowWidth - 1, Console.CursorTop + 1);
            }

            Console.ResetColor();
        }

        public void Update(int topOffset, int leftOffset, int positionFromTop, int positionFromLeft)
        {
            _topOffset = topOffset;
            _leftOffset = leftOffset;
            _positionFromTop = positionFromTop;
            _positionFromLeft = positionFromLeft;
        }
    }
}