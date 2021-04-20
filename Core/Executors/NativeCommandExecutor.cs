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
            _logger.Debug($"Start execution of the native command ({command.Content})");
            
            try
            {
                NativeCommander.GetExecutingFunction(command)();
            }
            catch (SourceCodeCompilationException error)
            {
                ErrorHandler.ShowError(error.Message);
            }
        }
    }
}
