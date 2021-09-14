using System;
using Domain.Core.Iterators;
using Domain.Core;

namespace DynamicEditor.Core.Iterators
{
    internal class DynamicConsoleInputFlowIterator : IInputFlowIterator
    {
        public bool HasNext()
            => true;

        public string Next()
        {
            var pressedKeys = Console.ReadKey(true);

            return pressedKeys.ToKeyCombination();
        }
    }
}
