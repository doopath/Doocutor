using System;
using Doocutor.Core.Exceptions;

namespace Doocutor.Core.Commands
{
    internal class NativeCommand : ICommand
    {
        public CommandType Type { get; private set; } = CommandType.NATIVE_COMMAND;
        public string Content { get; private set; }

        public NativeCommand(string content)
        {
            this.Content = content;
        }

        public int GetTargetLineNumber()
        {
            var message = $"Cannot take target line number of command {Content}";

            try
            {
                return int.Parse(Content.Split(" ")[1]);
            }
            catch (FormatException)
            {
                throw new CommandExecutingException(message);
            }
            catch (IndexOutOfRangeException)
            {
                throw new CommandExecutingException(message);
            }
        }

        public string GetTargetLine()
        {
            try
            {
                return string.Join(' ', Content.Trim().Split(" ")[2..]);
            }
            catch (IndexOutOfRangeException)
            {
                throw new CommandExecutingException(
                    $"Cannot take target line of command {Content}");
            }
        }

        public string[] GetArguments()
        {
            var parts = Content.Trim().Split(" ");

            return parts.Length < 2 ? Array.Empty<string>() : parts[1..];
        }
    }
}
