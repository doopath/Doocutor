using CommandHandling.Commands;
using Common;
using Utils.Exceptions;
using CUI;
using TextBuffer.TextBuffers;
using CUI.Widgets;
using TextBuffer;
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
            onOk: (_) => throw new InterruptedExecutionException(
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
        => TextBuffer!.AppendLine(TextBufferSettings.Tab);

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

    public static void ExecuteSaveCurrentBufferCommand(EditorCommand command)
    {
        if (TextBuffer!.FilePath is null)
        {
            bool terminate = false;
            
            WidgetsMount.Mount(new TextInputDialogWidget(
                text: "Your current buffer is untitled! Please enter a path to save it:",
                onCancel: () => terminate = true,
                onOk: path => TextBuffer.FilePath = TextBufferManager.ModifyPath((string) path!)));

            if (terminate) return;

                if (TextBuffer.FilePath is not null && !TextBufferManager.IsDirPathCorrect(TextBuffer.FilePath))
            {
                TextBuffer.FilePath = null;
                WidgetsMount.Mount(new AlertWidget("Entered path is incorrect! Cannot save the buffer D:"));
            }
        }
        
        if (TextBuffer.FilePath is not null)
            TextBufferManager.SaveTextBufferAsFile(TextBuffer);
        else
            WidgetsMount.Mount(new AlertWidget("Cannot save untitled buffer! D:", AlertLevel.ERROR));
    }

    public static void ExecuteOpenTextBufferCommand(EditorCommand command)
    {
        string filePath = string.Empty;
        bool terminate = false;
        
        WidgetsMount.Mount(new TextInputDialogWidget(
            text: "Enter file location:",
            onCancel: () => terminate = true,
            onOk: path => filePath = (string) path!));

        if (terminate) return;
        
        if (TextBufferManager.IsFilePathCorrect(filePath))
        {
            TextBufferManager.OpenAsTextBuffer(filePath, TextBuffer!);
            CuiRender.ScrollToTheTop();
        }
        else
            WidgetsMount.Mount(new AlertWidget(
                "Path is incorrect! Cannot open a buffer D:",
                AlertLevel.ERROR));
    }

    public static void ExecuteChangeCurrentFilePathCommand(EditorCommand command)
    {
        string filePath = string.Empty;
        bool terminate = false;
        
        WidgetsMount.Mount(new TextInputDialogWidget(
            text: "Enter new file path:",
            onCancel: () => terminate = true,
            onOk: path => filePath = (string) path!));

        if (terminate) return;
        
        filePath = TextBufferManager.ModifyPath(filePath);
        
        if (TextBufferManager.IsDirPathCorrect(filePath))
            TextBuffer!.FilePath = filePath;
        else
            WidgetsMount.Mount(new AlertWidget(
                $"Path is incorrect! Directory {TextBufferManager.GetDirectory(filePath)} doesn't exists!",
                AlertLevel.ERROR));
    }

    #endregion

    private static void CopyText(string text)
        => new Clipboard().SetText(text);
}
