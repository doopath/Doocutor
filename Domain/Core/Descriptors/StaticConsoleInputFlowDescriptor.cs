using System;
using Libraries.Core;

namespace Domain.Core.Descriptors
{
    public class StaticConsoleInputFlowDescriptor : IInputFlowDescriptor
    {
        public bool HasNext() => true;

        public string Next()
        {
            ShowArrows();

            return Console.ReadLine();
        }

        private void ShowArrows()
            => OutputColorizing.colorizeForeground(ConsoleColor.Cyan, () => Console.Write(">>> "));
    }
}
