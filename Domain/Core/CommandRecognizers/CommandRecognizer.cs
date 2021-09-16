using Domain.Core.Commands;
using Domain.Core.Exceptions;
using Domain.Core;
using Domain.Core.CommandRecognizers;

namespace Domain.Core.CommandRecognizers
{
    public sealed class CommandRecognizer : ICommandRecognizer
    {
        public ICommand Recognize(string command)
        {
            if (IsValidNativeCommand(command))
                return new NativeCommand(command);
           
            return new EditorCommand(command);
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

        private bool IsValidNativeCommand(string command)
        {
            command = command.Split(" ")[0];

            // It checks if command is a ":" because that can be the ternary operator.
            if (!command.StartsWith(":") || command == ":")
                return false;

            return (NativeCommandExecutionProvider.SupportedCommands.Contains(command))
                ? true
                : throw new CommandRecognizingException($"\"{command}\" is not a command!");
        }
    }
}
