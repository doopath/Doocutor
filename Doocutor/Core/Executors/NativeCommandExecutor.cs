using System;
using NLog;
using Doocutor.Core.Commands;
using Doocutor.Core.Exceptions;
using Libraries.Core;

namespace Doocutor.Core.Executors
{
    internal class NativeCommandExecutor : ICommandExecutor<NativeCommand>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void Execute(NativeCommand command)
        {
            Logger.Debug($"Start execution of the native command ({command.Content})");

            try
            {
                NativeCommandExecutionProvider.GetExecutingFunction(command)();
            }
            catch (SourceCodeCompilationException error)
            {
                Logger.Debug(error);
            }
            catch (Exception error) when (error.GetType() != typeof(InterruptedExecutionException))
            {
                ErrorHandler.showError(error);
            }
        }
    }
}
