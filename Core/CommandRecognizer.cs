using Doocutor.Core.Commands;
using Doocutor.Core.Exceptions;
using System;

namespace Doocutor.Core
{
    internal class CommandRecognizer : ICommandRecognizer
    {
        public ICommand Recognize(string command)
        {
            if (this.IsValidNativeCommand(command))
                return new NativeCommand(command);
            else
                return new EditorCommand(command);
        }

        private bool IsValidNativeCommand(string command)
        {
            // You can pass an expression as a command.
            // For example: <:writeAfter 5 Console.WriteLine("Hello world");>.
            // So it checks only first word.
            command = command.Split(" ")[0];

            if (command.StartsWith(":"))
            {
                if (NativeCommandsList.Contains(command))
                    return true;
                else
                    throw new CommandRecognizingException($"Gotten command \"{command}\" is incorrect!");
            }

            return false;
        }
    }
}
