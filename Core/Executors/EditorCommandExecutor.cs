using System;
using NLog;
using Doocutor.Core.Commands;

namespace Doocutor.Core.Executors
{
    internal class EditorCommandExecutor : ICommandExecutor<EditorCommand>
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        public void Execute(EditorCommand command)
        {
            _logger.Debug($"Start execution of the editor command ({command.Content})");

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
