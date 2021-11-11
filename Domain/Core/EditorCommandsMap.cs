using System.Collections.Generic;
using System.Linq;

namespace Domain.Core;

public static class EditorCommandsMap
{

    public static readonly Dictionary<string, ExecuteCommandDelegate> Map =
        new(new KeyValuePair<string, ExecuteCommandDelegate>[]
            {
                new(":quit", EditorCommandsList.ExecuteQuitCommand),
                new(":view", EditorCommandsList.ExecuteViewCommand),
                new(":write", EditorCommandsList.ExecuteWriteCommand),
                new(":writeAfter", EditorCommandsList.ExecuteWriteAfterCommand),
                new(":appendLine", EditorCommandsList.ExecuteAppendLineCommand),
                new(":enter", EditorCommandsList.ExecuteEnterCommand),
                new(":backspace", EditorCommandsList.ExecuteBackspaceCommand),
                new(":using", EditorCommandsList.ExecuteUsingCommand),
                new(":copy", EditorCommandsList.ExecuteCopyCommand),
                new(":copyAll", EditorCommandsList.ExecuteCopyAllCommand),
                new(":copyBlock", EditorCommandsList.ExecuteCopyBlockCommand),
                new(":clear", EditorCommandsList.ExecuteClearCommand),
                new(":remove", EditorCommandsList.ExecuteRemoveCommand),
                new(":removeBlock", EditorCommandsList.ExecuteRemoveBlockCommand),
                new(":replace", EditorCommandsList.ExecuteReplaceCommand),
                new(":set", EditorCommandsList.ExecuteSetCommand),
                new(":showPos", EditorCommandsList.ExecuteShowPosCommand),
                new(":saveCode", EditorCommandsList.ExecuteSaveCodeCommand),
                new(":help", EditorCommandsList.ExecuteHelpCommand),
                new(":info", EditorCommandsList.ExecuteInfoCommand),
                new(":tab", EditorCommandsList.ExecuteTabCommand),
                new(":undo", EditorCommandsList.ExecuteUndoCommand),
                new(":redo", EditorCommandsList.ExecuteRedoCommand),
                new(":doNothing", EditorCommandsList.ExecuteDoNothingCommand)
            }.ToList()
        );
}