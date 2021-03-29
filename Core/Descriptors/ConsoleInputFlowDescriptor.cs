using System;

namespace Doocutor.Core.Descriptors
{
    internal class ConsoleInputFlowDescriptor : IInputFlowDescriptor
    {
        public bool HasNext()
        {
            return true;
        }

        public string Next()
        {
            return Console.ReadLine();
        }
    }
}
