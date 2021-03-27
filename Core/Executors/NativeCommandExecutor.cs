using System;
using Doocutor.Core.Commands;

namespace Doocutor.Core.Executors
{
    class NativeCommandExecutor : ICommandExecutor
    {
        public void Execute(Command command)
        {
            // TODO: execute the gotten command.
            Console.WriteLine("Executing of a native command...");
        }
    }
}
