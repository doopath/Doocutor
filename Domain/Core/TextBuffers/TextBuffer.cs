using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Cursors;
using Domain.Core.Exceptions;
using Domain.Core.TextBufferContents;
using Domain.Core.TextBufferFormatters;
using Domain.Core.TextBufferHistories;
using Domain.Core.TextBuffers.TextPointers;

namespace Domain.Core.TextBuffers;

public class TextBuffer : ITextBuffer
{
    #region Private fields
    private readonly List<string> _sourceText;
    private readonly ITextBufferFormatter _codeFormatter;
    private readonly ICursor _cursor;
    private readonly ITextBufferHistory _history;
    private readonly KeyboardCommandsProvider _keyboardCommandsProvider;

    #endregion

    #region Constructors
    public TextBuffer(uint historyLimit = 10000)
    {
        var initialContentOfTheBuffer = new DefaultTextBufferContent();
        _sourceText = initialContentOfTheBuffer.SourceCode;
        _codeFormatter = new TextBufferFormatter(_sourceText);
        _history = new TextBufferHistory(historyLimit);
        _cursor = new TextBufferCursor(
            _sourceText,
            _codeFormatter,
            initialContentOfTheBuffer.CursorPositionFromTop,
            initialContentOfTheBuffer.CursorPositionFromLeft);
        _keyboardCommandsProvider = new(_sourceText, _codeFormatter, _cursor);
    }

    public TextBuffer(ITextBuffer buffer)
    {
        _sourceText = buffer.Lines.ToList();
        _codeFormatter = new TextBufferFormatter(_sourceText);
        _history = new TextBufferHistory(buffer.HistoryLimit);
        _cursor = new TextBufferCursor(
            _sourceText,
            _codeFormatter,
            buffer.CursorPositionFromTop,
            buffer.CursorPositionFromLeft);
        _keyboardCommandsProvider = new(_sourceText, _codeFormatter, _cursor);
    }

    public TextBuffer(ITextBufferContent initialContentOfTheBuffer, uint historyLimit = 100)
    {
        _sourceText = initialContentOfTheBuffer.SourceCode;
        _codeFormatter = new TextBufferFormatter(_sourceText);
        _history = new TextBufferHistory(historyLimit);
        _cursor = new TextBufferCursor(
            _sourceText,
            _codeFormatter,
            initialContentOfTheBuffer.CursorPositionFromTop,
            initialContentOfTheBuffer.CursorPositionFromLeft);
        _keyboardCommandsProvider = new(_sourceText, _codeFormatter, _cursor);
    }

    #endregion

    #region Public properties
    /// <summary>
    /// Max count of elements in the history.
    /// </summary>
    public uint HistoryLimit { get; set; }

    /// <summary>
    /// Count of lines in the buffer
    /// </summary>
    public int Size => _sourceText.Count;

    public int CursorPositionFromTop => _cursor.CursorPositionFromTop;

    public int CursorPositionFromLeft => _cursor.CursorPositionFromLeft;

    public string CurrentLine => GetCurrentLine();

    /// <summary>
    /// Get source code numerated by lines.
    /// For example then you do code in a text editor or and IDE you
    /// want to know numbers of lines for easier navigating.
    /// </summary>
    public string CodeWithLineNumbers => _codeFormatter.GetSourceCodeWithLineNumbers();

    /// <summary>
    /// Get bare code. Without line numbers and '\n' symbols.
    /// </summary>
    public string Code => _codeFormatter.GetSourceCode();

    /// <summary>
    /// Get bare code split by line ends.
    /// </summary>
    public string[] Lines => _sourceText.ToArray();

    #endregion

    #region public get methods
    public string GetLineAt(int lineNumber)
        => _codeFormatter.GetLineAt(lineNumber);

    public string[] GetCodeBlock(ITextBlockPointer pointer)
        => Lines
            .Skip(pointer.StartLineNumber - 1)
            .Take(pointer.EndLineNumber - pointer.StartLineNumber)
            .ToArray();

    public int GetPrefixLength()
        => _codeFormatter.GetPrefixLength(_codeFormatter.IndexToLineNumber(CursorPositionFromTop));

    #endregion

    #region Public remove methods
    public void RemoveCodeBlock(ITextBlockPointer pointer)
    {
        CheckIfLineExistsAt(_codeFormatter.LineNumberToIndex(pointer.EndLineNumber));

        for (var i = 0; i < pointer.EndLineNumber - pointer.StartLineNumber; i++)
        {
            _sourceText.RemoveAt(_codeFormatter.LineNumberToIndex(pointer.StartLineNumber));
        }

        AddLineIfBufferIsEmpty();
        SetPointerAtLastLineIfNecessary();
    }

    public void RemoveLineAt(int lineNumber)
    {
        CheckIfLineExistsAt(lineNumber);

        if (lineNumber == 1 && _sourceText.Count == 1)
            throw new OutOfCodeBufferSizeException($"Cannot remove the first line when the buffer's size is 1!");

        _sourceText.RemoveAt(_codeFormatter.LineNumberToIndex(lineNumber));
        SetPointerAtLastLineIfNecessary();
    }

    #endregion

    public void IncreaseBufferSize()
    {
        var lastIndex = _sourceText.Count - 1;
        _sourceText.Insert(lastIndex + 1, "");
    }

    public void AdaptCodeForBufferSize(int maxLineLength)
    {
        string[] oldSourceCode = _sourceText.ToArray();
        int initialCursorPositionFromLeft = CursorPositionFromLeft;
        int initialCursorPositionFromTop = CursorPositionFromTop;

        bool isModified = _codeFormatter.AdaptCodeForBufferSize(maxLineLength);

        if (!isModified)
            return;

        string[] newSourceCode = _sourceText.ToArray();
        CursorPosition initialCursorPosition = new(initialCursorPositionFromLeft, initialCursorPositionFromTop);
        CursorPosition cursorPosition = new(CursorPositionFromLeft, CursorPositionFromTop);

        _history.Add(new TextBufferChange()
        {
            Range = new(0, _sourceText.Count),
            OldCursorPosition = initialCursorPosition,
            NewCursorPosition = cursorPosition,
            NewChanges = newSourceCode,
            OldState = oldSourceCode
        });
    }


    #region Cursor movements
    public void SetCursorPositionFromTopAt(int position)
        => _cursor.SetCursorPositionFromTopAt(position);

    public void SetCursorPositionFromLeftAt(int position)
        => _cursor.SetCursorPositionFromLeftAt(position);

    public void IncCursorPositionFromLeft()
        => _cursor.IncCursorPositionFromLeft();

    public void DecCursorPositionFromLeft()
        => _cursor.DecCursorPositionFromLeft();

    public void IncCursorPositionFromTop()
        => _cursor.IncCursorPositionFromTop();

    public void DecCursorPositionFromTop()
        => _cursor.DecCursorPositionFromTop();

    #endregion

    public void ReplaceLineAt(int lineNumber, string newLine)
    {
        CheckIfLineExistsAt(lineNumber);
        _sourceText[_codeFormatter.LineNumberToIndex(lineNumber)] = newLine;
    }

    #region Public write methods
    public void Write(string line)
    {
        _sourceText.Insert(_cursor.CursorPositionFromTop, line);
        _cursor.IncCursorPositionFromTop();
    }

    public void WriteAfter(int lineNumber, string line)
    {
        CheckIfLineExistsAt(lineNumber);
        _sourceText.Insert(lineNumber, line);
    }

    public void WriteBefore(int lineNumber, string line)
    {
        if (_codeFormatter.LineNumberToIndex(lineNumber) > -1 && lineNumber <= _sourceText.Count)
            _sourceText.Insert(_codeFormatter.LineNumberToIndex(lineNumber), line);
        else
            throw new OutOfCodeBufferSizeException(
                $"You cannot write line before the line with line number = {lineNumber}!");
    }

    #endregion

    public void AppendLine(string newPart)
    {
        int initialCursorPositionFromLeft = CursorPositionFromLeft;
        int initialCursorPositionFromTop = CursorPositionFromTop;
        string line = _sourceText[CursorPositionFromTop];
        string newLine = _codeFormatter.SeparateLineFromLineNumber(
            _codeFormatter.GroupNewLineOfACurrentOne(newPart, CursorPositionFromTop, CursorPositionFromLeft));

        _sourceText[CursorPositionFromTop] = newLine;
        SetCursorPositionFromLeftAt(CursorPositionFromLeft + newPart.Length);

        _history.Add(new TextBufferChange()
        {
            Range = new(CursorPositionFromTop, CursorPositionFromTop + 1),
            OldCursorPosition = new CursorPosition(
                initialCursorPositionFromLeft, initialCursorPositionFromTop),
            NewCursorPosition = new CursorPosition(
                CursorPositionFromLeft, CursorPositionFromTop),
            NewChanges = new[] { newLine },
            OldState = new[] { line }
        });
    }

    public void Undo()
    {
        ITextBufferChange change;

        try
        {
            change = _history.Undo();
        }
        catch (ValueOutOfRangeException)
        {
            return;
        }

        int start = change.Range.Start.Value;
        int end = change.Range.End.Value;

        RemoveCodeBlock(new TextBlockPointer(
            _codeFormatter.IndexToLineNumber(start),
            _codeFormatter.IndexToLineNumber(end)));

        foreach (string line in change.OldState.Reverse())
            _sourceText.Insert(start, line);

        SetCursorPositionFromTopAt(change.OldCursorPosition.Top);
        SetCursorPositionFromLeftAt(change.OldCursorPosition.Left);
    }

    public void Redo()
    {
        ITextBufferChange change;

        try
        {
            change = _history.Redo();
        }
        catch (ValueOutOfRangeException)
        {
            return;
        }

        int start = change.Range.Start.Value;
        int end = change.Range.End.Value;

        RemoveCodeBlock(new TextBlockPointer(
            _codeFormatter.IndexToLineNumber(start),
            _codeFormatter.IndexToLineNumber(end)));

        foreach (string line in change.NewChanges.Reverse())
            _sourceText.Insert(start, line);

        SetCursorPositionFromTopAt(change.NewCursorPosition.Top);
        SetCursorPositionFromLeftAt(change.NewCursorPosition.Left);
    }

    #region Keyboard action
    public void Enter()
        => _keyboardCommandsProvider.Enter();

    public void Backspace()
        => _keyboardCommandsProvider.Backspace();

    #endregion

    #region Private Methods
    private void CheckIfLineExistsAt(int lineNumber)
    {
        if (_sourceText.Count < lineNumber || lineNumber < 1)
        {
            throw new OutOfCodeBufferSizeException($"Line number {lineNumber} does not exist!");
        }
    }

    private void SetPointerAtLastLineIfNecessary()
        => _cursor.CursorPositionFromTop = _cursor.CursorPositionFromTop <= _sourceText.Count
            ? _cursor.CursorPositionFromTop
            : _sourceText.Count;

    private void AddLineIfBufferIsEmpty()
    {
        if (_sourceText.Count == 0)
            _sourceText.Add("");
    }

    private string GetCurrentLine()
        => _sourceText.Count > 0 ? _sourceText[CursorPositionFromTop] : "";
    #endregion
}
