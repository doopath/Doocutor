using CommandHandling.Commands;
using Common;
using Utils.Exceptions;
using CUI;
using Domain.Core;
using TextBuffer.TextBuffers;
using CUI.Widgets;
using TextCopy;
using Utils;

namespace CommandHandling;

public static class EditorCommands
{
    public static ITextBuffer? TextBuffer { get; set; }

    public static void InitializeCodeBuffer(ITextBuffer buffer)
    {
        TextBuffer = buffer;
    }

    #region Commands
    public static void ExecuteQuitCommand(EditorCommand command)
    {
        WidgetsMount.Mount(new DialogWidget(
            text: "Are you sure you want to quit from the Doocutor?",
            onCancel: null,
            onOk: () => throw new InterruptedExecutionException(
                "You have came out of the doocutor! Good bye!")));
    }

    public static void ExecuteViewCommand(EditorCommand command)
    {
        Console.Clear();
        Console.WriteLine(TextBuffer!.CodeWithLineNumbers + "\n");
    }

    public static void ExecuteWriteAfterCommand(EditorCommand command)
        => TextBuffer!.WriteAfter(command.GetFirstArgumentAsAnInteger(), command.GetArgumentsSinceSecondAsALine());

    public static void ExecuteUsingCommand(EditorCommand command)
    {
        if (TextBuffer!.Text.Trim().StartsWith("namespace"))
            TextBuffer.WriteBefore(1, "");

        TextBuffer.WriteBefore(1, $"using {command.GetArguments()[0]};");
    }

    public static void ExecuteCopyCommand(EditorCommand command)
        => CopyText(TextBuffer!.GetLineAt(command.GetFirstArgumentAsAnInteger()));

    public static void ExecuteCopyAllCommand(EditorCommand command)
        => CopyText(TextBuffer!.Text);

    public static void ExecuteCopyBlockCommand(EditorCommand command)
        => CopyText(string.Join("\n", TextBuffer!.GetTextBlock(
            new TextBlockPointer(int.Parse(command.GetArguments()[0]), int.Parse(command.GetArguments()[1])))));

    public static void ExecuteRemoveCommand(EditorCommand command)
    {
        CopyText(TextBuffer!.GetLineAt(command.GetFirstArgumentAsAnInteger()));
        TextBuffer.RemoveLineAt(command.GetFirstArgumentAsAnInteger());
    }

    public static void ExecuteRemoveBlockCommand(EditorCommand command)
    {
        var arguments = command.GetArguments();
        var pointer = new TextBlockPointer(int.Parse(arguments[0]), int.Parse(arguments[1]));
        TextBuffer!.RemoveTextBlock(pointer);
    }

    public static void ExecuteReplaceCommand(EditorCommand command)
    {
        var arguments = command.GetArguments();
        TextBuffer!.ReplaceLineAt(int.Parse(arguments[0]), command.GetArgumentsSinceSecondAsALine());
    }

    public static void ExecuteWriteCommand(EditorCommand command)
        => TextBuffer!.Write(command.GetArgumentsAsALine());

    public static void ExecuteSetCommand(EditorCommand command)
        => TextBuffer!.SetCursorPositionFromTopAt(int.Parse(command.GetArguments()[0]));

    public static void ExecuteShowPosCommand(EditorCommand command)
    {
        Console.Write("Current cursor position: ");
        OutputColorizing.ColorizeForeground(ConsoleColor.Cyan,
           () => Console.Write(TextBuffer!.CursorPositionFromTop + "\n"));
    }

    public static void ExecuteHelpCommand(EditorCommand command)
        => HelpList.Show();

    public static void ExecuteInfoCommand(EditorCommand command)
        => Console.WriteLine(Info.Description);

    public static void ExecuteAppendLineCommand(EditorCommand command)
        => TextBuffer!.AppendLine(string.Join(" ", command.Content.Split(" ")[1..]));

    public static void ExecuteEnterCommand(EditorCommand command)
        => TextBuffer!.Enter();

    public static void ExecuteBackspaceCommand(EditorCommand command)
        => TextBuffer!.Backspace();

    public static void ExecuteTabCommand(EditorCommand command)
        => TextBuffer!.AppendLine("    ");

    public static void ExecuteDoNothingCommand(EditorCommand command) { }

    public static void ExecuteUndoCommand(EditorCommand command)
        => TextBuffer!.Undo();

    public static void ExecuteRedoCommand(EditorCommand command)
        => TextBuffer!.Redo();

    public static void ExecutePasteTextCommand(EditorCommand command)
    {
        string clipboardContent = new Clipboard().GetText() ?? "";

        TextBuffer!.PasteText(clipboardContent);
    }

    public static void ExecuteSelectLinesCommand(EditorCommand command)
    {
        WidgetsMount.Mount(new LinesSelectionWidget(TextBuffer!));
    }

    #endregion

    private static void CopyText(string text)
        => new Clipboard().SetText(text);
}
