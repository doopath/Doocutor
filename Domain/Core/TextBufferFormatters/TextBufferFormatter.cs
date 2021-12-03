using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Domain.Core.Exceptions;

namespace Domain.Core.TextBufferFormatters;

public class TextBufferFormatter : ITextBufferFormatter
{
    public List<string> SourceCode { get; }
    public int? MaxLineLength { get; private set; }

    public TextBufferFormatter(List<string> sourceCode)
    {
        SourceCode = sourceCode;
    }

    public bool AdaptCodeForBufferSize(int windowWidth)
    {
        MaxLineLength = windowWidth;
        bool isModified = false;

        for (var i = 0; i < SourceCode.Count; i++)
        {
            var lineNumber = i + 1;
            var line = GetLineAt(lineNumber);
            var prefixLength = GetPrefixLength(lineNumber);

            if (line.Length + prefixLength > windowWidth)
            {
                SourceCode[i] = line[..(windowWidth - prefixLength)];

                if (SourceCode.Count - 1 < i + 1)
                    SourceCode.Insert(i + 1, "");

                SourceCode[i + 1] = line[(windowWidth - prefixLength)..] + SourceCode[i + 1];

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

        return $"  {lineNumber}{GetOutputSpacesForLineAt(lineNumber)}|{SourceCode[LineNumberToIndex(lineNumber)]}\n";
    }
    public string SeparateLineFromLineNumber(string line)
        => string.Join("|", line.Split("|")[1..]);

    public string GetSourceCodeWithLineNumbers()
        => string.Join("", SourceCode.Select((_, i) => GroupOutputLineAt(IndexToLineNumber(i))))[..^1];

    public string GetLineAt(int lineNumber)
    {
        CheckIfLineExistsAt(lineNumber);
        return SourceCode[LineNumberToIndex(lineNumber)];
    }

    public string GetSourceCode()
        => string.Join("\n", SourceCode);

    public int IndexToLineNumber(int index) => index + 1;

    public int LineNumberToIndex(int lineNumber) => lineNumber - 1;

    public int GetPrefixLength(int currentLineNumber)
    {
        CheckIfLineExistsAt(currentLineNumber);

        var lastLineNumber = SourceCode.Count;
        var lastLineContent = SourceCode[LineNumberToIndex(lastLineNumber)];
        var lastLineContentLength = lastLineContent.Length;
        var lineNumberLength = (int)Math.Log10(lastLineNumber) + 1; // +1 because of log10(n) where n < 10 is a number < 1.
        var lastLineLength = 4 + lastLineContentLength + lineNumberLength;

        return lastLineLength - lastLineContentLength;
    }

    private bool IsClosingBrace(string line)
        => line.Equals("}");

    private bool LineHasOpeningBrace(string line)
    {
        RemoveAllButBracesIn(ref line);
        RemoveAllCoupleBracesIn(ref line);

        return IsOpeningBrace(line);
    }

    private bool LineHasClosingBrace(string line)
    {
        RemoveAllButBracesIn(ref line);
        RemoveAllCoupleBracesIn(ref line);

        return IsClosingBrace(line);
    }

    private bool IsOpeningBrace(string line)
        => line.Equals("{");

    private string RemoveAllButBracesIn(ref string line)
        => line = Regex.Replace(line, @"[^{}]", string.Empty);

    private void RemoveAllCoupleBracesIn(ref string line)
    {
        while (LineContainsBraces(line))
            line = RemoveCoupleBracesIn(ref line);
    }

    private string RemoveCoupleBracesIn(ref string line)
        => line = line.Replace(@"{}", string.Empty);

    private bool LineContainsBraces(string line)
        => line.Contains("{") && line.Contains(("}"));

    private string GetOutputSpacesForLineAt(int lineNumber)
        => ' ' + new string(' ', GetTimesOfSpacesRepeationForLineAt(lineNumber));

    private int GetTimesOfSpacesRepeationForLineAt(int lineNumber)
        => SourceCode.Count.ToString().Length - lineNumber.ToString().Length;

    private void CheckIfLineExistsAt(int lineNumber)
    {
        if (SourceCode.Count < lineNumber || lineNumber < 1)
        {
            throw new OutOfCodeBufferSizeException($"Line number {lineNumber} does not exist!");
        }
    }
}
