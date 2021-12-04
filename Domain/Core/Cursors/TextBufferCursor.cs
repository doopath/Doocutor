using System.Collections.Generic;
using Domain.Core.Exceptions;
using Domain.Core.TextBufferFormatters;

namespace Domain.Core.Cursors;

public sealed class TextBufferCursor : ICursor
{
    private readonly List<string> _sourceText;
    private readonly ITextBufferFormatter _formatter;
    public int CursorPositionFromLeft { get; set; }
    public int CursorPositionFromTop { get; set; }

    public TextBufferCursor(
        List<string> sourceCode,
        ITextBufferFormatter formatter,
        int initialPositionFromTop,
        int initialPositionFromLeft)
    {
        _sourceText = sourceCode;
        _formatter = formatter;
        CursorPositionFromLeft = initialPositionFromLeft;
        CursorPositionFromTop = initialPositionFromTop;
    }

    public void SetCursorPositionFromTopAt(int position)
    {
        CheckIfLineExistsAt(_formatter.IndexToLineNumber(position));

        var currentLineNumber = _formatter.IndexToLineNumber(CursorPositionFromTop);
        var prefixLength = _formatter.GetPrefixLength(currentLineNumber);
        var targetLineNumber = _formatter.IndexToLineNumber(position);
        var targetLine = _formatter.GroupOutputLineAt(targetLineNumber);
        var targetLineContent = _formatter.GetLineAt(targetLineNumber);

        CursorPositionFromTop = position;

        if (CursorPositionFromLeft == prefixLength && targetLineContent != "")
            CursorPositionFromLeft = prefixLength;
        else if (targetLineContent == "")
            CursorPositionFromLeft = prefixLength;
        else if (CursorPositionFromLeft >= targetLine.Length)
            CursorPositionFromLeft = targetLine.Length - 1;
        else if (CursorPositionFromLeft < prefixLength)
            CursorPositionFromLeft = prefixLength;
    }

    public void SetCursorPositionFromLeftAt(int position)
    {
        var currentLineNumber = _formatter.IndexToLineNumber(CursorPositionFromTop);
        var currentLine = _formatter.GroupOutputLineAt(currentLineNumber);
        var prefixLength = _formatter.GetPrefixLength(currentLineNumber);

        if (LineLengthOverflowedByCursor(position, currentLine) && NextLineOfTheBufferExists())
        {
            var nextLineNumber = currentLineNumber + 1;

            CursorPositionFromLeft = _formatter.GetPrefixLength(nextLineNumber);
            CursorPositionFromTop++;
        }
        else if (LineLengthOverflowedByCursor(position, currentLine) && IsThisTheLastLine())
        {
            return;
        }
        else if (position <= prefixLength - 1 && IsThisNotAFirstLine())
        {
            var previousLine = _formatter.GroupOutputLineAt(CursorPositionFromTop);

            CursorPositionFromTop--;
            CursorPositionFromLeft = previousLine.Length - 1;
        }
        else if (position <= prefixLength - 1 && IsThisAFirstLine())
        {
            return;
        }
        else
        {
            CursorPositionFromLeft = position;
        }
    }


    #region Change position of the cursor by 1

    public void IncCursorPositionFromLeft()
        => SetCursorPositionFromLeftAt(CursorPositionFromLeft + 1);

    public void DecCursorPositionFromLeft()
        => SetCursorPositionFromLeftAt(CursorPositionFromLeft - 1);

    public void IncCursorPositionFromTop()
        => SetCursorPositionFromTopAt(CursorPositionFromTop + 1);

    public void DecCursorPositionFromTop()
        => SetCursorPositionFromTopAt(CursorPositionFromTop - 1);

    #endregion

    private void CheckIfLineExistsAt(int lineNumber)
    {
        if (_sourceText.Count < lineNumber || lineNumber < 1)
            throw new OutOfCodeBufferSizeException($"Line number {lineNumber} does not exist!");
    }


    #region Condition simplifications

    private bool LineLengthOverflowedByCursor(int position, string currentLine)
        => position > currentLine.Length - 1;

    private bool NextLineOfTheBufferExists()
        => CursorPositionFromTop < _sourceText.Count - 1;

    private bool IsThisTheLastLine()
        => CursorPositionFromTop == _sourceText.Count - 1;

    private bool IsThisNotAFirstLine()
        => CursorPositionFromTop > 0;

    private bool IsThisAFirstLine()
        => CursorPositionFromTop == 0;

    #endregion
}
