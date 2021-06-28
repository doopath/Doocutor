using Doocutor.Core.Commands;
using Doocutor.Core.Exceptions;

namespace Doocutor.Core.CommandRecognizers
{
    internal class CommandRecognizer : ICommandRecognizer
    {
        public ICommand Recognize(string command)
        {
            if (IsValidNativeCommand(command))
                return new NativeCommand(command);
            else
                return new EditorCommand(command);
        }

        private bool IsValidNativeCommand(string command)
        {
            command = command.Split(" ")[0];

            // It checks if command is a ":" because that can be a ternary operator.
            if (!command.StartsWith(":") || command == ":")
                return false;

            return (NativeCommandExecutionProvider.SupportedCommands.Contains(command))
                ? true
                : throw new CommandRecognizingException($"\"{command}\" is not a command!");
        }
    }
}
