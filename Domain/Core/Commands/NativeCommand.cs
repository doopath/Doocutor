using System;
using Domain.Core.Exceptions;

namespace Domain.Core.Commands
{
    public class NativeCommand : ICommand
    {
        public CommandType Type { get; private set; } = CommandType.NATIVE_COMMAND;
        public string Content { get; private set; }

        public NativeCommand(string content)
        {
            Content = content;
        }

        public int GetFirstArgumentAsAnInteger()
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

        public string GetArgumentsSinceSecondAsALine()
        {
            try
            {
                return string.Join(" ", GetArguments()[1..]);
            }
            catch (IndexOutOfRangeException)
            {
                throw new CommandExecutingException($"Cannot take target line of command {Content}");
            }
        }

        public string[] GetArguments()
        {
            var parts = Content.Trim().Split(" ");

            return parts.Length < 2 ? Array.Empty<string>() : parts[1..];
        }

        public string GetArgumentsAsALine()
            => string.Join(" ", GetArguments());
    }
}
