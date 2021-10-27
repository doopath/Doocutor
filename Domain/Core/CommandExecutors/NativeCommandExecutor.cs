using System;
using NLog;
using Domain.Core.Commands;
using Domain.Core.Exceptions;
using Libraries.Core;

namespace Domain.Core.Executors
{
    public class NativeCommandExecutor : ICommandExecutor<NativeCommand>
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
                ErrorHandling.showError(error);
            }
        }
    }
}
