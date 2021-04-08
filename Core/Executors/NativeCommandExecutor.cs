using System;
using NLog;
using Doocutor.Core.Commands;
using Doocutor.Core.Exceptions;

namespace Doocutor.Core.Executors
{
    internal class NativeCommandExecutor : ICommandExecutor<NativeCommand>
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Execute(NativeCommand command)
        {
            _logger.Debug($"Starting of the native command ({command.Content}) execution.");
            
            try
            {
                NativeCommander.GetExecutingFunction(command)();
            }
            catch (SourceCodeCompilationException error)
            {
                OutputColorizer.ColorizeForeground(ConsoleColor.Red, () => Console.WriteLine(error.Message));
            }
        }
    }
}
