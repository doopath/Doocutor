using System;
using System.Collections.Generic;
using System.Linq;

using Doocutor.Core.Commands;
using Doocutor.Core.Exceptions;
using Doocutor.Core.CodeBuffers;

namespace Doocutor.Core
{
    internal delegate void executeCommandDelegate(NativeCommand command);

    internal class NativeCommander
    {
        private static readonly ICodeBuffer _sourceCode = new SourceCodeBuffer();
        public static readonly List<string> SupprotedCommands = new(Enumerable.ToList<string>(new string[]
        {
            ":quit",
            ":view",
            ":compile",
            ":run",
            ":using",
            ":writeAfter",
            ":remove",
            ":removeBlock",
            ":replace"
        }));

        public static Action GetExecutingFunction(NativeCommand command)
        {
            var function = commandsMap[command.Content.Split(" ")[0]] ?? throw new
                UnsupportedCommandException($"Given command {command.Content} is not supported!");

            return () => function.Invoke(command);
        }

        private static readonly Dictionary<string, executeCommandDelegate> commandsMap =
            new(Enumerable.ToList<KeyValuePair<string, executeCommandDelegate>>(new KeyValuePair<string, executeCommandDelegate>[]
                {
                    new KeyValuePair<string, executeCommandDelegate>(":quit", ExecuteQuitCommand),
                    new KeyValuePair<string, executeCommandDelegate>(":view", ExecuteViewCommand),
                    new KeyValuePair<string, executeCommandDelegate>(":writeAfter", ExecuteWriteAfterCommand)
                })
            );

        private static void ExecuteQuitCommand(NativeCommand command)
            => throw new InterruptedExecutionException("You have came out of the doocutor! Good bye!");

        private static void ExecuteViewCommand(NativeCommand command)
        {
            Console.Clear();
            Console.WriteLine(_sourceCode.Code);
        }

        private static void ExecuteWriteAfterCommand(NativeCommand command)
        {
            var argumentsLen = command.Content.Split(" ")[1..].Length;

            if (argumentsLen < 2)
                throw new CommandExecutingException(
                    $"Command :writeAfter takes 2 arguments (lineNumber and newLine) but got {argumentsLen}!");

            _sourceCode.WriteAfter(command.GetTargetLineNumber(), command.GetTargetLine());
        }
    }

    internal static class NativeCommandExtension
    {
        public static int GetTargetLineNumber(this NativeCommand command)
        {
            var message = $"Cannot take target line number of command {command.Content}";

            try
            {
                return int.Parse(command.Content.Split(" ")[1]);
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

        public static string GetTargetLine(this NativeCommand command)
        {
            try
            {
                return string.Join(' ', command.Content.Trim().Split(" ")[2..]);
            } catch (IndexOutOfRangeException)
            {
                throw new CommandExecutingException(
                    $"Cannot take target line of command {command.Content}");
            }
        }
    }
}