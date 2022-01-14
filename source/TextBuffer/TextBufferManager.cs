using Common;
using Microsoft.VisualBasic;
using TextBuffer.TextBufferContents;
using Utils.Exceptions.NotExitExceptions;

namespace TextBuffer;

public static class TextBufferManager
{
    public static ITextBuffer OpenAsTextBuffer(string path, int historyLimit = 10000)
    {
        string fileContent = File.ReadAllText(path);
        List<string> fileContentLines = fileContent.Split("\n").ToList();
        ITextBufferContent textBufferContent = new TextBufferContent()
        {
            SourceCode = fileContentLines,
            CursorPositionFromLeft = 0,
            CursorPositionFromTop = 0
        };
        ITextBuffer textBuffer = new TextBuffers.TextBuffer(textBufferContent, historyLimit);
        textBuffer.SetCursorPositionFromLeftAt(textBuffer.GetPrefixLength());

        return textBuffer;
    }

    public static void SaveTextBufferAsFile(ITextBuffer textBuffer, string? path = null)
    {
        if (path is null && textBuffer.FilePath is null)
            throw new TextBufferSavingException("Cannot save the textBuffer because of file path is not set!");

        string[] textBufferContent = textBuffer.Lines;

        File.WriteAllLinesAsync(path ?? textBuffer.FilePath!, textBufferContent);
    }

    public static bool IsPathCorrect(string path)
    {
        char separator = Path.DirectorySeparatorChar;
        string dirPath = string
            .Join(separator, ModifyPath(path).Split(separator)[..^1]);

        return Directory.Exists(dirPath);
    }

    public static string ModifyPath(string path)
    {
        char separator = Path.DirectorySeparatorChar;
        string homeDir = (Environment.GetEnvironmentVariable("userdir")
                          ?? Environment.GetEnvironmentVariable("HOME"))!
                         + separator;

        return path
            .Replace("~/", homeDir)
            .Replace("~\\", homeDir);
    }
}