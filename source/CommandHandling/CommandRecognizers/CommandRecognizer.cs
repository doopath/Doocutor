using CommandHandling.Commands;
using Utils.Exceptions.NotExitExceptions;

namespace CommandHandling.CommandRecognizers;

public sealed class CommandRecognizer : ICommandRecognizer
{
    public ICommand Recognize(string command)
    {
        if (IsValidEditorCommand(command))
            return new EditorCommand(command);

        throw new CommandRecognizingException($"Cannot recognize command '{command}'");
    }

    public ICommand? TryRecognize(string command)
    {
        try
        {
            return Recognize(command);
        }
        catch (CommandRecognizingException)
        {
            return null;
        }
    }

    private static bool IsValidEditorCommand(string command)
        => EditorCommandExecutionProvider
            .SupportedCommands
            .Contains(command.Split(" ")[0]);
}
