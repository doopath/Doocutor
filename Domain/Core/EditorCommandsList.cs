using System;
using Domain.Core.Cache;
using Domain.Core.Commands;
using Domain.Core.Exceptions;
using Domain.Core.TextBuffers;
using Domain.Core.TextBuffers.TextPointers;
using Libraries.Core;
using TextCopy;

namespace Domain.Core;

public static class EditorCommandsList
{
    public static ITextBuffer? SourceCodeBuffer { get; set; }

    public static void InitializeCodeBuffer(ref ITextBuffer buffer)
    {
        SourceCodeBuffer = buffer;
    }

    #region Commands
    public static void ExecuteQuitCommand(EditorCommand command)
    {
        ExecuteClearCommand(command);
        throw new InterruptedExecutionException("You have came out of the doocutor! Good bye!");
    }

    public static void ExecuteViewCommand(EditorCommand command)
    {
        Console.Clear();
        Console.WriteLine(SourceCodeBuffer!.CodeWithLineNumbers + "\n");
    }

    public static void ExecuteWriteAfterCommand(EditorCommand command)
        => SourceCodeBuffer!.WriteAfter(command.GetFirstArgumentAsAnInteger(), command.GetArgumentsSinceSecondAsALine());

    public static void ExecuteClearCommand(EditorCommand command)
        => Console.Clear();

    public static void ExecuteUsingCommand(EditorCommand command)
    {
        if (SourceCodeBuffer!.Code.Trim().StartsWith("namespace"))
            SourceCodeBuffer.WriteBefore(1, "");

        SourceCodeBuffer.WriteBefore(1, $"using {command.GetArguments()[0]};");
    }

    public static void ExecuteCopyCommand(EditorCommand command)
        => CopyText(SourceCodeBuffer!.GetLineAt(command.GetFirstArgumentAsAnInteger()));

    public static void ExecuteCopyAllCommand(EditorCommand command)
        => CopyText(SourceCodeBuffer!.Code);

    public static void ExecuteCopyBlockCommand(EditorCommand command)
        => CopyText(string.Join("\n", SourceCodeBuffer!.GetCodeBlock(
            new TextBlockPointer(int.Parse(command.GetArguments()[0]), int.Parse(command.GetArguments()[1])))));

    public static void ExecuteRemoveCommand(EditorCommand command)
    {
        CopyText(SourceCodeBuffer!.GetLineAt(command.GetFirstArgumentAsAnInteger()));
        SourceCodeBuffer.RemoveLineAt(command.GetFirstArgumentAsAnInteger());
    }

    public static void ExecuteRemoveBlockCommand(EditorCommand command)
    {
        var arguments = command.GetArguments();
        var pointer = new TextBlockPointer(int.Parse(arguments[0]), int.Parse(arguments[1]));
        SourceCodeBuffer!.RemoveTextBlock(pointer);
    }

    public static void ExecuteReplaceCommand(EditorCommand command)
    {
        var arguments = command.GetArguments();
        SourceCodeBuffer!.ReplaceLineAt(int.Parse(arguments[0]), command.GetArgumentsSinceSecondAsALine());
    }

    public static void ExecuteWriteCommand(EditorCommand command)
        => SourceCodeBuffer!.Write(command.GetArgumentsAsALine());

    public static void ExecuteSetCommand(EditorCommand command)
        => SourceCodeBuffer!.SetCursorPositionFromTopAt(int.Parse(command.GetArguments()[0]));

    public static void ExecuteShowPosCommand(EditorCommand command)
    {
        Console.Write("Current cursor position: ");
        OutputColorizing.colorizeForeground(ConsoleColor.Cyan,
            () => Console.Write(SourceCodeBuffer!.CursorPositionFromTop + "\n"));
    }

    public static void ExecuteSaveCodeCommand(EditorCommand command)
        => SourceCodeSaving.saveCode(command.GetArgumentsAsALine(), SourceCodeBuffer!.Lines);

    public static void ExecuteHelpCommand(EditorCommand command)
        => HelpList.Show();

    public static void ExecuteInfoCommand(EditorCommand command)
        => Console.WriteLine(Info.Description);

    public static void ExecuteAppendLineCommand(EditorCommand command)
        => SourceCodeBuffer!.AppendLine(string.Join(" ", command.Content.Split(" ")[1..]));

    public static void ExecuteEnterCommand(EditorCommand command)
        => SourceCodeBuffer!.Enter();

    public static void ExecuteBackspaceCommand(EditorCommand command)
        => SourceCodeBuffer!.Backspace();

    public static void ExecuteTabCommand(EditorCommand command)
        => SourceCodeBuffer!.AppendLine("    ");

    public static void ExecuteDoNothingCommand(EditorCommand command) { }

    public static void ExecuteUndoCommand(EditorCommand command)
        => SourceCodeBuffer!.Undo();

    public static void ExecuteRedoCommand(EditorCommand command)
        => SourceCodeBuffer!.Redo();

    #endregion

    private static void CopyText(string text)
        => new Clipboard().SetText(text);
}
