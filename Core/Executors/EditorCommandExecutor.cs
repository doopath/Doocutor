using System;
using Doocutor.Core.Commands;

namespace Doocutor.Core.Executors
{
    class EditorCommandExecutor : ICommandExecutor
    {
        public void Execute(Command command)
        {
            // TODO: execute the gotten command.
            Console.WriteLine("Executing of an editor command...");
        }
    }
}
