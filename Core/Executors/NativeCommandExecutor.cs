using System;
using Doocutor.Core.Commands;

namespace Doocutor.Core.Executors
{
    internal class NativeCommandExecutor : ICommandExecutor<NativeCommand>
    {
        public void Execute(NativeCommand command)
        {
            // TODO: execute the gotten command.
            Console.WriteLine("Executing of a native command...");
        }
    }
}
