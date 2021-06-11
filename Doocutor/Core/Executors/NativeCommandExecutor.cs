using System;
using System.Data;
using NLog;
using Doocutor.Core.Commands;
using Doocutor.Core.Exceptions;

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
            catch (Exception error) when (error.GetType() != typeof(InterruptedExecutionException))
            {
                ErrorHandler.ShowError(error);
            }
        }
    }
}
