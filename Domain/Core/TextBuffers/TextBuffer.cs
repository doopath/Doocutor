using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Cursors;
using Domain.Core.Exceptions;
using Domain.Core.TextBufferContents;
using Domain.Core.TextBufferFormatters;
using Domain.Core.TextBufferHistories;
using Domain.Core.TextBuffers.TextPointers;
using Domain.Core.Widgets;

namespace Domain.Core.TextBuffers;

public class TextBuffer : ITextBuffer
{
    #region Protected fields
    protected readonly List<string> _sourceText;
    protected readonly ITextBufferFormatter _codeFormatter;
    protected readonly ICursor _cursor;
    protected readonly ITextBufferHistory _history;
    protected readonly KeyboardCommandsProvider _keyboardCommandsProvider;

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
    public virtual uint HistoryLimit { get; set; }

    /// <summary>
    /// Count of lines in the buffer
    /// </summary>
    public virtual int Size => _sourceText.Count;

    public virtual int CursorPositionFromTop => _cursor.CursorPositionFromTop;

    public virtual int CursorPositionFromLeft => _cursor.CursorPositionFromLeft;

    public virtual string CurrentLine => GetCurrentLine();

    /// <summary>
    /// Get source code numerated by lines.
    /// For example then you do code in a text editor or and IDE you
    /// want to know numbers of lines for easier navigating.
    /// </summary>
    public virtual string CodeWithLineNumbers => _codeFormatter.GetSourceCodeWithLineNumbers();

    /// <summary>
    /// Get bare code. Without line numbers and '\n' symbols.
    /// </summary>
    public virtual string Code => _codeFormatter.GetSourceCode();

    /// <summary>
    /// Get bare code split by line ends.
    /// </summary>
    public virtual string[] Lines => _sourceText.ToArray();

    #endregion

    #region Public get methods
    public virtual string GetLineAt(int lineNumber)
        => _codeFormatter.GetLineAt(lineNumber);

    public virtual string[] GetCodeBlock(ITextBlockPointer pointer)
        => Lines
            .Skip(pointer.StartLineNumber - 1)
            .Take(pointer.EndLineNumber - pointer.StartLineNumber)
            .ToArray();

    public virtual int GetPrefixLength()
        => _codeFormatter.GetPrefixLength(_codeFormatter.IndexToLineNumber(CursorPositionFromTop));

    #endregion

    #region Public remove methods
    public virtual void RemoveTextBlock(ITextBlockPointer pointer)
    {
        CheckIfLineExistsAt(_codeFormatter.LineNumberToIndex(pointer.EndLineNumber));

        for (var i = 0; i < pointer.EndLineNumber - pointer.StartLineNumber; i++)
        {
            _sourceText.RemoveAt(_codeFormatter.LineNumberToIndex(pointer.StartLineNumber));
        }

        AddLineIfBufferIsEmpty();
        SetPointerAtLastLineIfNecessary();
    }

    public virtual void RemoveLineAt(int lineNumber)
    {
        CheckIfLineExistsAt(lineNumber);

        if (lineNumber == 1 && _sourceText.Count == 1)
            throw new OutOfCodeBufferSizeException($"Cannot remove the first line when the buffer's size is 1!");

        _sourceText.RemoveAt(_codeFormatter.LineNumberToIndex(lineNumber));
        SetPointerAtLastLineIfNecessary();
    }
    #endregion

    public virtual void IncreaseBufferSize()
    {
        var lastIndex = _sourceText.Count - 1;
        _sourceText.Insert(lastIndex + 1, "");
    }

    public virtual void AdaptTextForBufferSize(int maxLineLength)
    {
        string[] oldSourceCode = _sourceText.ToArray();
        int initialCursorPositionFromLeft = CursorPositionFromLeft;
        int initialCursorPositionFromTop = CursorPositionFromTop;
        int initialPrefixLength = GetPrefixLength();

        if (_codeFormatter.AdaptCodeForBufferSize(maxLineLength))
            while (_codeFormatter.AdaptCodeForBufferSize(maxLineLength))
                continue;
        else
            return;

        string[] newSourceCode = _sourceText.ToArray();
        int prefixLengthDiff = initialPrefixLength - GetPrefixLength();

        SetCursorPositionFromLeftAt(CursorPositionFromLeft + prefixLengthDiff);
        
        CursorPosition initialCursorPosition = new()
        {
            Left = initialCursorPositionFromLeft,
            Top = initialCursorPositionFromTop
        };
        CursorPosition cursorPosition = new()
        {
            Left = CursorPositionFromLeft,
            Top = CursorPositionFromTop
        };

        _history.Add(new TextBufferChange()
        {
            Range = new(0, _sourceText.Count),
            OldCursorPosition = initialCursorPosition,
            NewCursorPosition = cursorPosition,
            NewChanges = newSourceCode,
            OldState = oldSourceCode,
            Type = TextBufferChangeType.ADAPT_TEXT
        });
    }


    #region Cursor movements
    public virtual void SetCursorPositionFromTopAt(int position)
        => _cursor.SetCursorPositionFromTopAt(position);

    public virtual void SetCursorPositionFromLeftAt(int position)
        => _cursor.SetCursorPositionFromLeftAt(position);

    public virtual void IncCursorPositionFromLeft()
        => _cursor.IncCursorPositionFromLeft();

    public virtual void DecCursorPositionFromLeft()
        => _cursor.DecCursorPositionFromLeft();

    public virtual void IncCursorPositionFromTop()
        => _cursor.IncCursorPositionFromTop();

    public virtual void DecCursorPositionFromTop()
        => _cursor.DecCursorPositionFromTop();

    #endregion

    public virtual void ReplaceLineAt(int lineNumber, string newLine)
    {
        CheckIfLineExistsAt(lineNumber);
        _sourceText[_codeFormatter.LineNumberToIndex(lineNumber)] = newLine;
    }

    #region Public write methods
    public virtual void Write(string line)
    {
        _sourceText.Insert(_cursor.CursorPositionFromTop, line);
        _cursor.IncCursorPositionFromTop();
    }

    public virtual void WriteAfter(int lineNumber, string line)
    {
        CheckIfLineExistsAt(lineNumber);
        _sourceText.Insert(lineNumber, line);
    }

    public virtual void WriteBefore(int lineNumber, string line)
    {
        if (_codeFormatter.LineNumberToIndex(lineNumber) > -1 && lineNumber <= _sourceText.Count)
            _sourceText.Insert(_codeFormatter.LineNumberToIndex(lineNumber), line);
        else
            throw new OutOfCodeBufferSizeException(
                $"You cannot write line before the line with line number = {lineNumber}!");
    }

    #endregion

    public virtual void AppendLine(string newPart)
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
            OldCursorPosition = new()
            {
                Left = initialCursorPositionFromLeft,
                Top = initialCursorPositionFromTop
            },
            NewCursorPosition = new()
            {
                Left = CursorPositionFromLeft,
                Top = CursorPositionFromTop
            },
            NewChanges = new[] { newLine },
            OldState = new[] { line },
            Type = TextBufferChangeType.APPEND_LINE
        });
    }

    public virtual void Undo()
    {
        ITextBufferChange change;

        try
        {
            change = _history.Undo();

            if (change.Type is TextBufferChangeType.ADAPT_TEXT)
            {
                UndoAdaptText(change);
                Undo();
                return;
            }
        }
        catch (ValueOutOfRangeException)
        {
            WidgetsMount.Mount(new AlertWidget("You are already at the oldest change!"));
            return;
        }

        int start = change.Range.Start.Value;
        int end = change.Range.End.Value;

        _sourceText.RemoveRange(start, end - start);
        _sourceText.InsertRange(start, change.OldState);
        SetCursorPositionFromTopAt(change.OldCursorPosition.Top);
        SetCursorPositionFromLeftAt(change.OldCursorPosition.Left);
    }

    public virtual void Redo()
    {
        ITextBufferChange change;

        try
        {
            change = _history.Redo();
        }
        catch (ValueOutOfRangeException)
        {
            WidgetsMount.Mount(new AlertWidget("You are already at the latest change!"));
            return;
        }

        int start = change.Range.Start.Value;
        int end = change.Range.End.Value;

        _sourceText.RemoveRange(start, end - start);
        _sourceText.InsertRange(start, change.NewChanges);
        SetCursorPositionFromTopAt(change.NewCursorPosition.Top);
        SetCursorPositionFromLeftAt(change.NewCursorPosition.Left);
    }

    #region Keyboard action
    public virtual void Enter()
        => _keyboardCommandsProvider.Enter();

    public virtual void Backspace()
        => _keyboardCommandsProvider.Backspace();

    #endregion

    #region Protected Methods
    protected virtual void UndoAdaptText(ITextBufferChange change)
    {
        if (change.Type is not TextBufferChangeType.ADAPT_TEXT)
            throw new InvalidOperationException(
                $"change.Type is not {nameof(TextBufferChangeType.ADAPT_TEXT)}!");

        _sourceText.Clear();
        _sourceText.AddRange(change.OldState);
    }

    protected virtual void CheckIfLineExistsAt(int lineNumber)
    {
        if (_sourceText.Count < lineNumber || lineNumber < 1)
            throw new OutOfCodeBufferSizeException($"Line number {lineNumber} does not exist!");
    }

    protected virtual void SetPointerAtLastLineIfNecessary()
        => _cursor.CursorPositionFromTop = _cursor.CursorPositionFromTop <= _sourceText.Count
            ? _cursor.CursorPositionFromTop
            : _sourceText.Count;

    protected virtual void AddLineIfBufferIsEmpty()
    {
        if (_sourceText.Count == 0)
            _sourceText.Add("");
    }

    protected virtual string GetCurrentLine()
        => _sourceText.Count > 0 ? _sourceText[CursorPositionFromTop] : "";
    #endregion
}
