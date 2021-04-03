using System;
using System.Collections.Generic;
using System.Linq;

using Doocutor.Core.Commands;
using Doocutor.Core.Exceptions;

namespace Doocutor.Core
{
    internal delegate void executeCommandDelegate(NativeCommand command);

    internal class NativeCommander
    {
        public static readonly List<string> SupprotedCommands = new(Enumerable.ToList<string>(new string[]
        {
            ":quit",
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
            var function = commandsMap[command.Content] ?? throw new
                UnsupportedCommandException($"Given command {command.Content} is not supported!");

            return () => function.Invoke(command);
        }

        private static readonly Dictionary<string, executeCommandDelegate> commandsMap =
            new(Enumerable.ToList<KeyValuePair<string, executeCommandDelegate>>(new KeyValuePair<string, executeCommandDelegate>[]
                {
                    new KeyValuePair<string, executeCommandDelegate>(":quit", ExecuteQuitCommand),
                    new KeyValuePair<string, executeCommandDelegate>(":compile", (NativeCommand command) => { })
                })
            );

        private static void ExecuteQuitCommand(NativeCommand command)
        {
            throw new InterruptedExecutionException("You have came out of the doocutor! Good bye!");
        }
    }
}
