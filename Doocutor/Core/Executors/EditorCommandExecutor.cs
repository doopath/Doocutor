using System;
using NLog;
using Doocutor.Core.Commands;
using DoocutorLibraries.Core;

using Error = DoocutorLibraries.Core.Common.Error;

namespace Doocutor.Core.Executors
{
    internal class EditorCommandExecutor : ICommandExecutor<EditorCommand>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        public void Execute(EditorCommand command)
        {
            Logger.Debug($"Start execution of the editor command ({command.Content})");

            try
            {
                NativeCommander.GetExecutingFunction(new NativeCommand(":write " + command.Content.Trim()))();
            }
            catch (Exception error)
            {
                ErrorHandler.showError(Error.NewException(error));
            }
        }
    }
}
