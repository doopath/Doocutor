using System;
using Libraries.Core;
using Domain.Core.Iterators;

namespace StaticEditor.Core.Iterators
{
    internal class StaticConsoleInputFlowIterator : IInputFlowIterator
    {
        public bool HasNext() => true;

        public string Next()
        {
            ShowArrows();

            return Console.ReadLine();
        }

        private static void ShowArrows()
            => OutputColorizing.colorizeForeground(ConsoleColor.Cyan, () => Console.Write(">>> "));
    }
}
