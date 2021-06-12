using System;
using NLog;
using Doocutor.Core.Commands;
using Doocutor.Core.Exceptions;
using DoocutorLibraries.Core;

using Error = DoocutorLibraries.Core.Common.Error;

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
                NativeCommander.GetExecutingFunction(command)();
            }
            catch (SourceCodeCompilationException error)
            {
                Logger.Debug(error);
            }
            catch (Exception error) when (error.GetType() != typeof(InterruptedExecutionException))
            {
                ErrorHandler.showError(Error.NewException(error));
            }
        }
    }
}
