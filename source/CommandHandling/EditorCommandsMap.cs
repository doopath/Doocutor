namespace CommandHandling;

public static class EditorCommandsMap
{

    public static readonly Dictionary<string, ExecuteCommandDelegate> Map =
        new(new KeyValuePair<string, ExecuteCommandDelegate>[]
            {
                new(":quit", EditorCommands.ExecuteQuitCommand),
                new(":view", EditorCommands.ExecuteViewCommand),
                new(":write", EditorCommands.ExecuteWriteCommand),
                new(":writeAfter", EditorCommands.ExecuteWriteAfterCommand),
                new(":appendLine", EditorCommands.ExecuteAppendLineCommand),
                new(":pasteClipboardContent", EditorCommands.ExecutePasteTextCommand),
                new(":selectLines", EditorCommands.ExecuteSelectLinesCommand),
                new(":enter", EditorCommands.ExecuteEnterCommand),
                new(":backspace", EditorCommands.ExecuteBackspaceCommand),
                new(":using", EditorCommands.ExecuteUsingCommand),
                new(":copy", EditorCommands.ExecuteCopyCommand),
                new(":copyAll", EditorCommands.ExecuteCopyAllCommand),
                new(":copyBlock", EditorCommands.ExecuteCopyBlockCommand),
                new(":remove", EditorCommands.ExecuteRemoveCommand),
                new(":removeBlock", EditorCommands.ExecuteRemoveBlockCommand),
                new(":replace", EditorCommands.ExecuteReplaceCommand),
                new(":set", EditorCommands.ExecuteSetCommand),
                new(":showPos", EditorCommands.ExecuteShowPosCommand),
                new(":help", EditorCommands.ExecuteHelpCommand),
                new(":info", EditorCommands.ExecuteInfoCommand),
                new(":tab", EditorCommands.ExecuteTabCommand),
                new(":undo", EditorCommands.ExecuteUndoCommand),
                new(":redo", EditorCommands.ExecuteRedoCommand),
                new(":saveCurrentBuffer", EditorCommands.ExecuteSaveCurrentBufferCommand),
                new(":openTextBuffer", EditorCommands.ExecuteOpenTextBufferCommand),
                new(":changeCurrentFilePath", EditorCommands.ExecuteChangeCurrentFilePathCommand),
                new(":doNothing", EditorCommands.ExecuteDoNothingCommand)
            }.ToList()
        );
}