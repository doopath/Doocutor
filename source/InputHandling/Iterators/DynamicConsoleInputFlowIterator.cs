using Domain.Core.Iterators;

namespace InputHandling.Iterators
{
    public class DynamicConsoleInputFlowIterator : IInputFlowIterator
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
