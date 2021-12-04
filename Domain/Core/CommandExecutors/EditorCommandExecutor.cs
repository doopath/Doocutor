using System;
using Domain.Core.Commands;
using Domain.Core.Exceptions;
using NLog;

namespace Domain.Core.Executors;

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
        catch (Exception error) when (error.GetType() != typeof(InterruptedExecutionException))
        {
            ErrorHandling.ShowError(error);
        }
    }
}
