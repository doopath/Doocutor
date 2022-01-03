using System;
using Domain.Core.Iterators;

namespace DynamicEditor.Core.Iterators
{
    internal class DynamicConsoleInputFlowIterator : IInputFlowIterator
    {
        public bool HasNext()
            => true;

        public ConsoleKeyInfo Next()
        {
            var pressedKeys = Console.ReadKey(true);

            return pressedKeys;
        }

        public string NextLine()
            => Console.ReadLine()!;
    }
}
