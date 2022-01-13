using CommandHandling.Commands;
using Utils.Exceptions;
using Utils.Exceptions.NotExitExceptions;
using CUI;
using NLog;

namespace CommandHandling.CommandExecutors;

public class EditorCommandExecutor : ICommandExecutor<EditorCommand>
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public void Execute(EditorCommand command)
    {
        Logger.Debug($"Start execution of the native command ({command.Content})");

        try
        {
            EditorCommandExecutionProvider.GetExecutingFunction(command)();
        }
        catch (SourceCodeCompilationException error)
        {
            Logger.Debug(error);
        }
        catch (NotExitException error)
        {
            ErrorHandling.ShowError(error);
        }
        catch (Exception error) when (error.GetType() != typeof(InterruptedExecutionException))
        {
            ErrorHandling.ShowError(error);
        }
    }
}
