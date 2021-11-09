using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using Domain.Core.CodeBufferContents;
using Domain.Core.CodeBufferHistories;
using Domain.Core.CodeBuffers.CodePointers;
using Domain.Core.CodeFormatters;
using Domain.Core.Cursors;
using Domain.Core.Exceptions;

namespace Domain.Core.CodeBuffers;

public class SourceCodeBuffer : ICodeBuffer
{
    #region Private fields
    private readonly List<string> _sourceCode;
    private readonly ICodeFormatter _codeFormatter;
    private readonly ICursor _cursor;
    private readonly ICodeBufferHistory _history;
    private readonly KeyboardCommandsProvider _keyboardCommandsProvider;

    #endregion

    #region Constructors
    public SourceCodeBuffer(uint historyLimit = 10000)
    {
        var initialContentOfTheBuffer = new InitialSourceCodeBufferContent();
        _sourceCode = initialContentOfTheBuffer.SourceCode;
        _codeFormatter = new SourceCodeFormatter(_sourceCode);
        _history = new SourceCodeBufferHistory(historyLimit);
        _cursor = new CodeBufferCursor(
            _sourceCode,
            _codeFormatter,
            initialContentOfTheBuffer.CursorPositionFromTop,
            initialContentOfTheBuffer.CursorPositionFromLeft);
        _keyboardCommandsProvider = new(_sourceCode, _codeFormatter, _cursor);
    }

    public SourceCodeBuffer(ICodeBuffer buffer)
    {
        _sourceCode = buffer.Lines.ToList();
        _codeFormatter = new SourceCodeFormatter(_sourceCode);
        _history = new SourceCodeBufferHistory(buffer.HistoryLimit);
        _cursor = new CodeBufferCursor(
            _sourceCode,
            _codeFormatter,
            buffer.CursorPositionFromTop,
            buffer.CursorPositionFromLeft);
        _keyboardCommandsProvider = new(_sourceCode, _codeFormatter, _cursor);
    }

    public SourceCodeBuffer(ICodeBufferContent initialContentOfTheBuffer, uint historyLimit = 100)
    {
        _sourceCode = initialContentOfTheBuffer.SourceCode;
        _codeFormatter = new SourceCodeFormatter(_sourceCode);
        _history = new SourceCodeBufferHistory(historyLimit);
        _cursor = new CodeBufferCursor(
            _sourceCode,
            _codeFormatter,
            initialContentOfTheBuffer.CursorPositionFromTop,
            initialContentOfTheBuffer.CursorPositionFromLeft);
        _keyboardCommandsProvider = new(_sourceCode, _codeFormatter, _cursor);
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
    public int Size => _sourceCode.Count;

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
    public string[] Lines => _sourceCode.ToArray();

    #endregion

    #region public get methods
    public string GetLineAt(int lineNumber)
        => _codeFormatter.GetLineAt(lineNumber);

    public string[] GetCodeBlock(ICodeBlockPointer pointer)
        => Lines
            .Skip(pointer.StartLineNumber - 1)
            .Take(pointer.EndLineNumber - pointer.StartLineNumber)
            .ToArray();

    public int GetPrefixLength()
        => _codeFormatter.GetPrefixLength(_codeFormatter.IndexToLineNumber(CursorPositionFromTop));

    #endregion

    #region Public remove methods
    public void RemoveCodeBlock(ICodeBlockPointer pointer)
    {
        CheckIfLineExistsAt(_codeFormatter.LineNumberToIndex(pointer.EndLineNumber));

        for (var i = 0; i < pointer.EndLineNumber - pointer.StartLineNumber; i++)
        {
            _sourceCode.RemoveAt(_codeFormatter.LineNumberToIndex(pointer.StartLineNumber));
        }

        AddLineIfBufferIsEmpty();
        SetPointerAtLastLineIfNecessary();
    }

    public void RemoveLineAt(int lineNumber)
    {
        CheckIfLineExistsAt(lineNumber);

        if (lineNumber == 1 && _sourceCode.Count == 1)
            throw new OutOfCodeBufferSizeException($"Cannot remove the first line when the buffer's size is 1!");

        _sourceCode.RemoveAt(_codeFormatter.LineNumberToIndex(lineNumber));
        SetPointerAtLastLineIfNecessary();
    }

    #endregion

    public void IncreaseBufferSize()
    {
        var lastIndex = _sourceCode.Count - 1;
        _sourceCode.Insert(lastIndex + 1, "");
    }

    public void AdaptCodeForBufferSize(int maxLineLength)
    {
        _codeFormatter.AdaptCodeForBufferSize(maxLineLength);
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
        _sourceCode[_codeFormatter.LineNumberToIndex(lineNumber)] = _codeFormatter.ModifyLine(newLine, lineNumber);
    }

    #region Public write methods
    public void Write(string line)
    {
        _sourceCode.Insert(
            _cursor.CursorPositionFromTop,
            _codeFormatter.ModifyLine(line, _codeFormatter.IndexToLineNumber(_cursor.CursorPositionFromTop)));
        _cursor.IncCursorPositionFromTop();
    }

    public void WriteAfter(int lineNumber, string line)
    {
        CheckIfLineExistsAt(lineNumber);
        _sourceCode.Insert(lineNumber, _codeFormatter.ModifyLine(line, lineNumber));
    }

    public void WriteBefore(int lineNumber, string line)
    {
        if (_codeFormatter.LineNumberToIndex(lineNumber) > -1 && lineNumber <= _sourceCode.Count)
            _sourceCode.Insert(_codeFormatter.LineNumberToIndex(lineNumber), _codeFormatter.ModifyLine(line, 1));
        else
            throw new OutOfCodeBufferSizeException(
                $"You cannot write line before the line with line number = {lineNumber}!");
    }

    #endregion

    public void AppendLine(string newPart)
    {
        int initialCursorPositionFromLeft = CursorPositionFromLeft;
        int initialCursorPositionFromTop = CursorPositionFromTop;
        string line = _sourceCode[CursorPositionFromTop];
        string newLine = _codeFormatter.SeparateLineFromLineNumber(
            _codeFormatter.GroupNewLineOfACurrentOne(newPart, CursorPositionFromTop, CursorPositionFromLeft));

        _sourceCode[CursorPositionFromTop] = newLine;
        SetCursorPositionFromLeftAt(CursorPositionFromLeft + newPart.Length);

        _history.Add(new CodeBufferChange()
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
        ICodeBufferChange change;

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

        RemoveCodeBlock(new CodeBlockPointer(
            _codeFormatter.IndexToLineNumber(start),
            _codeFormatter.IndexToLineNumber(end)));

        foreach (string line in change.OldState.Reverse())
            _sourceCode.Insert(start, line);

        SetCursorPositionFromTopAt(change.OldCursorPosition.Top);
        SetCursorPositionFromLeftAt(change.OldCursorPosition.Left);
    }

    public void Redo()
    {
        ICodeBufferChange change;

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

        RemoveCodeBlock(new CodeBlockPointer(
            _codeFormatter.IndexToLineNumber(start),
            _codeFormatter.IndexToLineNumber(end)));

        foreach (string line in change.NewChanges.Reverse())
            _sourceCode.Insert(start, line);

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
        if (_sourceCode.Count < lineNumber || lineNumber < 1)
        {
            throw new OutOfCodeBufferSizeException($"Line number {lineNumber} does not exist!");
        }
    }

    [SupportedOSPlatform("windows")]
    private bool IsUpperCaseSymbolOnWindows(string key)
        => Regex.IsMatch(key, @"Shift\+[\w]", RegexOptions.IgnoreCase) || Console.CapsLock;

    [SupportedOSPlatform("windows")]
    private bool IsLowerCaseSymbolOnWindows(string key)
        => Regex.IsMatch(key, "[A-Z]", RegexOptions.IgnoreCase) && !Console.CapsLock;

    private bool IsUpperCaseSymbol(string key)
        => Regex.IsMatch(key, @"Shift\+[\w]", RegexOptions.IgnoreCase);

    private bool IsLowerCaseSymbol(string key)
        => Regex.IsMatch(key, "[A-Z]", RegexOptions.IgnoreCase);

    private string KeyToUpperCaseSymbol(string key)
        => key.Replace("Shift+", "").ToUpper();

    private void SetPointerAtLastLineIfNecessary()
        => _cursor.CursorPositionFromTop = _cursor.CursorPositionFromTop <= _sourceCode.Count
            ? _cursor.CursorPositionFromTop
            : _sourceCode.Count;

    private void AddLineIfBufferIsEmpty()
    {
        if (_sourceCode.Count == 0)
            _sourceCode.Add("");
    }

    private string GetCurrentLine()
        => _sourceCode.Count > 0 ? _sourceCode[CursorPositionFromTop] : "";
    #endregion
}
