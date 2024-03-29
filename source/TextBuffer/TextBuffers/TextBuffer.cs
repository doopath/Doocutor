﻿using Common;
using CUI;
using CUI.Widgets;
using TextBuffer.Cursors;
using TextBuffer.TextBufferContents;
using TextBuffer.TextBufferFormatters;
using TextBuffer.TextBufferHistories;
using Utils.Exceptions.NotExitExceptions;

namespace TextBuffer.TextBuffers;

public class TextBuffer : ITextBuffer
{
    #region Protected fields

    protected readonly List<string> _sourceText;
    protected readonly ITextBufferFormatter _textFormatter;
    protected readonly ICursor _cursor;
    protected readonly ITextBufferHistory _history;
    protected readonly KeyboardCommandsProvider _keyboardCommandsProvider;

    #endregion

    #region Public properties

    /// <summary>
    /// Max count of elements in the history.
    /// </summary>
    public virtual int HistoryLimit { get; set; }

    public virtual string? FilePath { get; set; }

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
    public virtual string CodeWithLineNumbers => _textFormatter.GetSourceCodeWithLineNumbers();

    /// <summary>
    /// Get bare code. Without line numbers and '\n' symbols.
    /// </summary>
    public virtual string Text => _textFormatter.GetSourceCode();

    /// <summary>
    /// Get bare code split by line ends.
    /// </summary>
    public virtual string[] Lines => _sourceText.ToArray();

    #endregion

    #region Public get methods

    public virtual string GetLineAt(int lineNumber)
        => _textFormatter.GetLineAt(lineNumber);

    public virtual string[] GetTextBlock(ITextBlockPointer pointer)
        => Lines
            .Skip(pointer.StartLineNumber - 1)
            .Take(pointer.EndLineNumber - pointer.StartLineNumber)
            .ToArray();

    public virtual int GetPrefixLength()
        => _textFormatter.GetPrefixLength(_textFormatter.IndexToLineNumber(CursorPositionFromTop));

    #endregion

    #region Constructors

    public TextBuffer(int historyLimit = 10000, string? filePath = null)
    {
        var initialContentOfTheBuffer = new DefaultTextBufferContent();
        FilePath = filePath;
        _sourceText = initialContentOfTheBuffer.SourceCode;
        _textFormatter = new TextBufferFormatter(_sourceText);
        _history = new TextBufferHistory(historyLimit);
        _cursor = new TextBufferCursor(
            _sourceText,
            _textFormatter,
            initialContentOfTheBuffer.CursorPositionFromTop,
            initialContentOfTheBuffer.CursorPositionFromLeft);
        _keyboardCommandsProvider = new(_sourceText, _textFormatter, _cursor);
    }

    public TextBuffer(ITextBuffer buffer, string? filePath = null)
    {
        FilePath = filePath;
        _sourceText = buffer.Lines.ToList();
        _textFormatter = new TextBufferFormatter(_sourceText);
        _history = new TextBufferHistory(buffer.HistoryLimit);
        _cursor = new TextBufferCursor(
            _sourceText,
            _textFormatter,
            buffer.CursorPositionFromTop,
            buffer.CursorPositionFromLeft);
        _keyboardCommandsProvider = new(_sourceText, _textFormatter, _cursor);
    }

    public TextBuffer(ITextBufferContent initialContentOfTheBuffer, int historyLimit = 10000, string? filePath = null)
    {
        FilePath = filePath;
        _sourceText = initialContentOfTheBuffer.SourceCode;
        _textFormatter = new TextBufferFormatter(_sourceText);
        _history = new TextBufferHistory(historyLimit);
        _cursor = new TextBufferCursor(
            _sourceText,
            _textFormatter,
            initialContentOfTheBuffer.CursorPositionFromTop,
            initialContentOfTheBuffer.CursorPositionFromLeft);
        _keyboardCommandsProvider = new(_sourceText, _textFormatter, _cursor);
    }

    #endregion

    #region Public remove methods

    public virtual void RemoveTextBlock(ITextBlockPointer pointer)
    {
        CheckIfLineExistsAt(pointer.StartLineNumber);
        CheckIfLineExistsAt(pointer.EndLineNumber - 1);

        int startIndex = _textFormatter.LineNumberToIndex(pointer.StartLineNumber);
        int linesToRemove = pointer.EndLineNumber - pointer.StartLineNumber;

        if (linesToRemove != Size)
        {
            SetCursorPositionFromLeftAt(_textFormatter.GetPrefixLength(_textFormatter.IndexToLineNumber(startIndex)));
            SetCursorPositionFromTopAt(startIndex > 0 ? startIndex - 1 : 0);
            _sourceText.RemoveRange(startIndex, linesToRemove);
        }
        else
        {
            int firstLineIndex = 0;
            int firstLineNumber = _textFormatter.IndexToLineNumber(firstLineIndex);

            _sourceText.Insert(firstLineIndex, string.Empty);
            _sourceText.RemoveRange(startIndex + 1, linesToRemove);
            _cursor.CursorPositionFromLeft = _textFormatter.GetPrefixLength(firstLineNumber);
            _cursor.CursorPositionFromTop = firstLineIndex;
        }
    }

    public virtual void RemoveLineAt(int lineNumber)
    {
        CheckIfLineExistsAt(lineNumber);

        if (lineNumber == 1 && _sourceText.Count == 1)
            throw new OutOfTextBufferSizeException($"Cannot remove the first line when the buffer's size is 1!");

        _sourceText.RemoveAt(_textFormatter.LineNumberToIndex(lineNumber));
        SetCursorAtLastLineIfNecessary();
    }

    public virtual void ClearHistory()
        => _history.Clear();

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

        if (_textFormatter.AdaptCodeForBufferSize(maxLineLength))
            while (_textFormatter.AdaptCodeForBufferSize(maxLineLength))
                continue;
        else
            return;

        string[] newSourceCode = _sourceText.ToArray();
        int prefixLengthDiff = GetPrefixLength() - initialPrefixLength;

        if (prefixLengthDiff > 0)
        {
            // Don't know exactly why, but +2 is ok and it works fine with that.
            // Otherwise the CursorPositionFromLeft will be incorrect (but only when
            // the AdaptTextForBufferSize makes more lines, than were before; for example:
            // There were 99 lines, but after adapting there are 100 lines. Prefix of the lines
            // was increased '  100 |' with length=7 instead of '  99 |' with length=6.)
            for (int i = prefixLengthDiff + 2; i > 0; i--)
                IncCursorPositionFromLeft();
        }

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
        _sourceText[_textFormatter.LineNumberToIndex(lineNumber)] = newLine;
    }

    public virtual void ReplaceCurrentContentBy(List<string> content)
    {
        content = content
            .Select(line => line
                .Replace("\t", TextBufferSettings.Tab)
                .Replace("    ", TextBufferSettings.Tab))
            .ToList();
        _sourceText.Clear();
        _sourceText.InsertRange(0, content);
        _cursor.CursorPositionFromTop = 0;
        _cursor.CursorPositionFromLeft = 0;
        SetCursorPositionFromLeftAt(GetPrefixLength());
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
        if (_textFormatter.LineNumberToIndex(lineNumber) > -1 && lineNumber <= _sourceText.Count)
            _sourceText.Insert(_textFormatter.LineNumberToIndex(lineNumber), line);
        else
            throw new OutOfTextBufferSizeException(
                $"You cannot write line before the line with line number = {lineNumber}!");
    }

    #endregion

    public virtual void PasteText(string text)
    {
        int initialPrefixLength = GetPrefixLength();
        IEnumerable<string> lines = text
            .Split("\n")
            .Reverse()
            .Select(line => line.Replace("\r", ""));

        foreach (string line in lines)
            _sourceText.Insert(CursorPositionFromTop, line);

        SetCursorPositionFromTopAt(CursorPositionFromTop + lines.Count());
        SetCursorPositionFromLeftAt(CursorPositionFromLeft + GetPrefixLength() - initialPrefixLength);
    }

    public virtual void AppendLine(string newPart)
    {
        int initialCursorPositionFromLeft = CursorPositionFromLeft;
        int initialCursorPositionFromTop = CursorPositionFromTop;
        string line = _sourceText[CursorPositionFromTop];
        string newLine = _textFormatter.SeparateLineFromLineNumber(
            _textFormatter.GroupNewLineOfACurrentOne(newPart, CursorPositionFromTop, CursorPositionFromLeft));

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

            if (change.Type is TextBufferChangeType.ADAPT_TEXT)
            {
                RedoAdaptText(change);
                Redo();
                return;
            }
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

    protected virtual void RedoAdaptText(ITextBufferChange change)
    {
        if (change.Type is not TextBufferChangeType.ADAPT_TEXT)
            throw new InvalidOperationException(
                $"change.Type is not {nameof(TextBufferChangeType.ADAPT_TEXT)}!");

        _sourceText.Clear();
        _sourceText.AddRange(change.NewChanges);
    }

    protected virtual void CheckIfLineExistsAt(int lineNumber)
    {
        if (_sourceText.Count < lineNumber || lineNumber < 1)
            throw new OutOfTextBufferSizeException($"Line number {lineNumber} does not exist!");
    }

    protected virtual void SetCursorAtLastLineIfNecessary()
    {
        _cursor.CursorPositionFromLeft = _textFormatter.GetPrefixLength(Size);
        _cursor.CursorPositionFromTop = CursorPositionFromTop <= Size
            ? Size - 1
            : CursorPositionFromTop;
    }

    protected virtual void AddLineIfBufferIsEmpty()
    {
        if (_sourceText.Count == 0)
            _sourceText.Add("");
    }

    protected virtual string GetCurrentLine()
        => _sourceText.Count > 0 ? _sourceText[CursorPositionFromTop] : "";
    #endregion
}