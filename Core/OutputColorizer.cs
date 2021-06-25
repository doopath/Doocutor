using System;

namespace Doocutor.Core
{
    internal static class OutputColorizer
    {
        private static readonly ConsoleColor _foreground = ConsoleColor.White;
        private static readonly ConsoleColor _backbround = ConsoleColor.Black;

        public static void ColorizeForeground(ConsoleColor color, Action action)
        {
            Console.ForegroundColor = color;
            action.Invoke();
            SetDefaultColors();
        }

        public static void ColorizeBackground(ConsoleColor color, Action action)
        {
            Console.BackgroundColor = color;
            action.Invoke();
            SetDefaultColors();
        }

        private static void SetDefaultColors()
        {
            Console.ForegroundColor = _foreground;
            Console.BackgroundColor = _backbround;
        }
    }
}
