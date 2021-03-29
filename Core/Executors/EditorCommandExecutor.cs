using System;
using Doocutor.Core.Commands;

namespace Doocutor.Core.Executors
{
    internal class EditorCommandExecutor : ICommandExecutor<EditorCommand>
    {
        public void Execute(EditorCommand command)
        {
            // TODO: execute the gotten command.
            Console.WriteLine("Executing of an editor command...");
        }
    }
}
