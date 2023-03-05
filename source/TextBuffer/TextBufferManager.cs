using Common;
using System.Text;
using Utils.Exceptions.NotExitExceptions;

namespace TextBuffer;

public static class TextBufferManager
{
    public static void OpenAsTextBuffer(string path, ITextBuffer textBuffer)
    {
        path = ModifyPath(path);
        string fileContent = File.ReadAllText(path, Encoding.UTF8).Replace("\r", "");
        List<string> fileContentLines = fileContent.Split("\n").ToList();

        textBuffer.ClearHistory();
        textBuffer.ReplaceCurrentContentBy(fileContentLines);
        textBuffer.FilePath = path;
    }

    public static void SaveTextBufferAsFile(ITextBuffer textBuffer, string? path = null)
    {
        if (path is null && textBuffer.FilePath is null)
            throw new TextBufferSavingException("Cannot save the textBuffer because of file path is not set!");

        using var fs = new FileStream(path ?? textBuffer.FilePath!, FileMode.Append);
        using var sw = new StreamWriter(stream: fs, encoding: Encoding.UTF8);

        foreach (var line in textBuffer.Lines)
            sw.WriteLine(line);
    }

    public static bool IsDirPathCorrect(string path)
        => Directory.Exists(GetDirectory(path));

    public static bool IsFilePathCorrect(string path)
        => File.Exists(ModifyPath(path));

    public static string ModifyPath(string path)
    {
        char separator = Path.DirectorySeparatorChar;
        string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + separator;

        return path
            .Replace("~/", homeDir)
            .Replace("~\\", homeDir);
    }

    public static string GetDirectory(string path)
    {
        char separator = Path.DirectorySeparatorChar;
        path = ModifyPath(path);

        if (Directory.Exists(path))
            return path;

        string cutPath = string.Join(separator, path.Split(separator)[..^1]);

        if (Directory.Exists(cutPath))
            return cutPath;

        return string.Empty;
    }
}
