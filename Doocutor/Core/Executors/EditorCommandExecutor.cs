using System;
using NLog;
using Doocutor.Core.Commands;

namespace Doocutor.Core.Executors
{
    internal class EditorCommandExecutor : ICommandExecutor<EditorCommand>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly NativeCommander Commander = new();
        
        public void Execute(EditorCommand command)
        {
            Logger.Debug($"Start execution of the editor command ({command.Content})");

            try
            {
                NativeCommander.GetExecutingFunction(new NativeCommand(":write " + command.Content.Trim()))();
            }
            catch (Exception error)
            {
                ErrorHandler.ShowError(error);
            }
        }
    }
}
