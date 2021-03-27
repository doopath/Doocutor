using System;

namespace Doocutor.Core
{
    class ConsoleInputFlowDescriptor : InputFlowDescriptor
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
