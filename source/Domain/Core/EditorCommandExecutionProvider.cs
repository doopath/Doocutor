using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Commands;
using Domain.Core.Exceptions.NotExitExceptions;

namespace Domain.Core;

public delegate void ExecuteCommandDelegate(EditorCommand command);

public class EditorCommandExecutionProvider
{
    /// <summary>
    /// Get a function (as an action) for a given command to execute that.
    /// </summary>
    /// Throws that if a given command is not included in the list of supported commands.
    /// </exception>
    public static Action GetExecutingFunction(EditorCommand command)
    {
        var content = command.Content.Split(" ")[0];

        if (SupportedCommands.Contains(content))
            return () => EditorCommandsMap.Map[content].Invoke(command);

        throw new UnsupportedCommandException($"Given command ({command.Content}) is not supported!");
    }

    public static readonly List<string> SupportedCommands = EditorCommandsMap.Map.Keys.ToList();
}