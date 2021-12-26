using System;
using Domain.Core.Cache;
using Domain.Core.Commands;
using Domain.Core.Exceptions.NotExitExceptions;
using Domain.Core.TextBuffers;
using Domain.Core.TextBuffers.TextPointers;
using Domain.Core.Widgets;
using TextCopy;

namespace Domain.Core;

public static class EditorCommands
{
    public static ITextBuffer? SourceTextBuffer { get; set; }

    public static void InitializeCodeBuffer(ITextBuffer buffer)
    {
        SourceTextBuffer = buffer;
    }

    #region Commands
    public static void ExecuteQuitCommand(EditorCommand command)
    {
        ExecuteClearCommand(command);
        WidgetsMount.Mount(new DialogWidget(
            text: "Are you sure you want to quit from the Doocutor?",
            onCancel: () => { },
            onOk: () => throw new InterruptedExecutionException(
                "You have came out of the doocutor! Good bye!")));
    }

    public static void ExecuteViewCommand(EditorCommand command)
    {
        Console.Clear();
        Console.WriteLine(SourceTextBuffer!.CodeWithLineNumbers + "\n");
    }

    public static void ExecuteWriteAfterCommand(EditorCommand command)
        => SourceTextBuffer!.WriteAfter(command.GetFirstArgumentAsAnInteger(), command.GetArgumentsSinceSecondAsALine());

    public static void ExecuteClearCommand(EditorCommand command)
        => Console.Clear();

    public static void ExecuteUsingCommand(EditorCommand command)
    {
        if (SourceTextBuffer!.Text.Trim().StartsWith("namespace"))
            SourceTextBuffer.WriteBefore(1, "");

        SourceTextBuffer.WriteBefore(1, $"using {command.GetArguments()[0]};");
    }

    public static void ExecuteCopyCommand(EditorCommand command)
        => CopyText(SourceTextBuffer!.GetLineAt(command.GetFirstArgumentAsAnInteger()));

    public static void ExecuteCopyAllCommand(EditorCommand command)
        => CopyText(SourceTextBuffer!.Text);

    public static void ExecuteCopyBlockCommand(EditorCommand command)
        => CopyText(string.Join("\n", SourceTextBuffer!.GetTextBlock(
            new TextBlockPointer(int.Parse(command.GetArguments()[0]), int.Parse(command.GetArguments()[1])))));

    public static void ExecuteRemoveCommand(EditorCommand command)
    {
        CopyText(SourceTextBuffer!.GetLineAt(command.GetFirstArgumentAsAnInteger()));
        SourceTextBuffer.RemoveLineAt(command.GetFirstArgumentAsAnInteger());
    }

    public static void ExecuteRemoveBlockCommand(EditorCommand command)
    {
        var arguments = command.GetArguments();
        var pointer = new TextBlockPointer(int.Parse(arguments[0]), int.Parse(arguments[1]));
        SourceTextBuffer!.RemoveTextBlock(pointer);
    }

    public static void ExecuteReplaceCommand(EditorCommand command)
    {
        var arguments = command.GetArguments();
        SourceTextBuffer!.ReplaceLineAt(int.Parse(arguments[0]), command.GetArgumentsSinceSecondAsALine());
    }

    public static void ExecuteWriteCommand(EditorCommand command)
        => SourceTextBuffer!.Write(command.GetArgumentsAsALine());

    public static void ExecuteSetCommand(EditorCommand command)
        => SourceTextBuffer!.SetCursorPositionFromTopAt(int.Parse(command.GetArguments()[0]));

    public static void ExecuteShowPosCommand(EditorCommand command)
    {
        Console.Write("Current cursor position: ");
        OutputColorizing.ColorizeForeground(ConsoleColor.Cyan,
           () => Console.Write(SourceTextBuffer!.CursorPositionFromTop + "\n"));
    }

    public static void ExecuteHelpCommand(EditorCommand command)
        => HelpList.Show();

    public static void ExecuteInfoCommand(EditorCommand command)
        => Console.WriteLine(Info.Description);

    public static void ExecuteAppendLineCommand(EditorCommand command)
        => SourceTextBuffer!.AppendLine(string.Join(" ", command.Content.Split(" ")[1..]));

    public static void ExecuteEnterCommand(EditorCommand command)
        => SourceTextBuffer!.Enter();

    public static void ExecuteBackspaceCommand(EditorCommand command)
        => SourceTextBuffer!.Backspace();

    public static void ExecuteTabCommand(EditorCommand command)
        => SourceTextBuffer!.AppendLine("    ");

    public static void ExecuteDoNothingCommand(EditorCommand command) { }

    public static void ExecuteUndoCommand(EditorCommand command)
        => SourceTextBuffer!.Undo();

    public static void ExecuteRedoCommand(EditorCommand command)
        => SourceTextBuffer!.Redo();

    public static void ExecutePasteTextCommand(EditorCommand command)
    {
        string clipboardContent = new Clipboard().GetText() ?? "";

        SourceTextBuffer!.PasteText(clipboardContent);
    }

    #endregion

    private static void CopyText(string text)
        => new Clipboard().SetText(text);
}
