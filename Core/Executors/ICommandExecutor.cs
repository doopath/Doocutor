using Doocutor.Core.Commands;

namespace Doocutor.Core.Executors
{
    /// <summary>
    /// Executor of input commands
    /// (for example: natime command - ":run" or some editor command).
    /// </summary>
    interface ICommandExecutor
    {
        void Execute(Command command);
    }
}
