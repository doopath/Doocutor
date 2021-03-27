using Doocutor.Core.Commands;
using Doocutor.Core.Exceptions;
using System;

namespace Doocutor.Core
{
    class CommandRecognizer : ICommandRecognizer
    {
        public Command Recognize(string command)
        {
            if (this.IsValidNativeCommand(command))
                return new NativeCommand(command);
            else
                return new EditorCommand(command);
        }

        private bool IsValidNativeCommand(string command)
        {
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
