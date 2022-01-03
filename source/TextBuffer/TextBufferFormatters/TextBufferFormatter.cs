using Utils.Exceptions.NotExitExceptions;

namespace TextBuffer.TextBufferFormatters;

public class TextBufferFormatter : ITextBufferFormatter
{
    public List<string> SourceText { get; }
    public int? MaxLineLength { get; private set; }

    public TextBufferFormatter(List<string> sourceText)
    {
        SourceText = sourceText;
    }

    public bool AdaptCodeForBufferSize(int windowWidth)
    {
        MaxLineLength = windowWidth;
        bool isModified = false;

        for (var i = 0; i < SourceText.Count; i++)
        {
            var lineNumber = i + 1;
            var line = GetLineAt(lineNumber);
            var prefixLength = GetPrefixLength(lineNumber);

            if (line.Length + prefixLength > windowWidth)
            {
                SourceText[i] = line[..(windowWidth - prefixLength)];

                if (SourceText.Count - 1 < i + 1)
                    SourceText.Insert(i + 1, "");

                SourceText[i + 1] = line[(windowWidth - prefixLength)..] + SourceText[i + 1];

                isModified = true;
            }
        }

        return isModified;
    }

    public string GroupNewLineOfACurrentOne(string newPart, int cursorPositionFromTop, int cursorPositionFromLeft)
    {
        var lineNumber = cursorPositionFromTop + 1;
        var currentLine = GroupOutputLineAt(lineNumber)[..^1];

        return currentLine.Insert(cursorPositionFromLeft, newPart);
    }

    public string GroupOutputLineAt(int lineNumber)
    {
        CheckIfLineExistsAt(lineNumber);

        return $"  {lineNumber}{GetOutputSpacesForLineAt(lineNumber)}|{SourceText[LineNumberToIndex(lineNumber)]}\n";
    }
    public string SeparateLineFromLineNumber(string line)
        => string.Join("|", line.Split("|")[1..]);

    public string GetSourceCodeWithLineNumbers()
        => string.Join("", SourceText.Select((_, i) => GroupOutputLineAt(IndexToLineNumber(i))))[..^1];

    public string GetLineAt(int lineNumber)
    {
        CheckIfLineExistsAt(lineNumber);
        return SourceText[LineNumberToIndex(lineNumber)];
    }

    public string GetSourceCode()
        => string.Join("\n", SourceText);

    public int IndexToLineNumber(int index)
        => index + 1;

    public int LineNumberToIndex(int lineNumber)
        => lineNumber - 1;

    public int GetPrefixLength(int currentLineNumber)
    {
        CheckIfLineExistsAt(currentLineNumber);

        var lastLineNumber = SourceText.Count;
        var lastLineContent = SourceText[LineNumberToIndex(lastLineNumber)];
        var lastLineContentLength = lastLineContent.Length;
        var lineNumberLength = (int)Math.Log10(lastLineNumber) + 1; // +1 because of log10(n) where n < 10 is a number < 1.
        var lastLineLength = 4 + lastLineContentLength + lineNumberLength;

        return lastLineLength - lastLineContentLength;
    }

    private string GetOutputSpacesForLineAt(int lineNumber)
        => ' ' + new string(' ', GetTimesOfSpacesRepeationForLineAt(lineNumber));

    private int GetTimesOfSpacesRepeationForLineAt(int lineNumber)
        => SourceText.Count.ToString().Length - lineNumber.ToString().Length;

    private void CheckIfLineExistsAt(int lineNumber)
    {
        if (SourceText.Count < lineNumber || lineNumber < 1)
            throw new OutOfTextBufferSizeException($"Line number {lineNumber} does not exist!");
    }
}
