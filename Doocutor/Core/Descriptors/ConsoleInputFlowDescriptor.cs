using System;
using DoocutorLibraries.Core;

namespace Doocutor.Core.Descriptors
{
    internal class ConsoleInputFlowDescriptor : IInputFlowDescriptor
    {
        public bool HasNext() => true;

        public string Next()
        {
            ShowArrows();

            return Console.ReadLine();
        }

        private void ShowArrows()
            => OutputColorizer.colorizeForeground(ConsoleColor.Cyan, () => Console.Write(">>> "));
    }
}
