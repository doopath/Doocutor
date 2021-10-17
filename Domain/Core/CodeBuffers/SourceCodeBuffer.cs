using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Domain.Core.CodeBufferContents;
using Domain.Core.CodeBuffers.CodePointers;
using Domain.Core.CodeFormatters;
using Domain.Core.Cursors;
using Domain.Core.Exceptions;

namespace Domain.Core.CodeBuffers
{
    public class SourceCodeBuffer : ICodeBuffer
    {
        private readonly List<string> _sourceCode;
        private readonly ICodeFormatter _codeFormatter;
        private readonly ICursor _cursor;
        private readonly KeyboardCommandsProvider _keyboardCommandsProvider;

        public SourceCodeBuffer()
        {
            var initialContentOfTheBuffer = new InitialSourceCodeBufferContent();
            _sourceCode = initialContentOfTheBuffer.SourceCode;
            _codeFormatter = new SourceCodeFormatter(_sourceCode);
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
            _cursor = new CodeBufferCursor(
                _sourceCode,
                _codeFormatter,
                buffer.CursorPositionFromTop,
                buffer.CursorPositionFromLeft);
            _keyboardCommandsProvider = new(_sourceCode, _codeFormatter, _cursor);
        }

        public SourceCodeBuffer(ICodeBufferContent initialContentOfTheBuffer)
        {
            _sourceCode = initialContentOfTheBuffer.SourceCode;
            _codeFormatter = new SourceCodeFormatter(_sourceCode);
            _cursor = new CodeBufferCursor(
                _sourceCode,
                _codeFormatter,
                initialContentOfTheBuffer.CursorPositionFromTop,
                initialContentOfTheBuffer.CursorPositionFromLeft);
            _keyboardCommandsProvider = new(_sourceCode, _codeFormatter, _cursor);
        }

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

        public string GetLineAt(int lineNumber) => _codeFormatter.GetLineAt(lineNumber);

        public string[] GetCodeBlock(ICodeBlockPointer pointer)
            => Lines[_codeFormatter.LineNumberToIndex(pointer.StartLineNumber).._codeFormatter.LineNumberToIndex(pointer.EndLineNumber)];

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

        public void IncreaseBufferSize()
        {
            var lastIndex = _sourceCode.Count - 1;
            _sourceCode.Insert(lastIndex + 1, "");
        }

        public int GetPrefixLength()
            => _codeFormatter.GetPrefixLength(_codeFormatter.IndexToLineNumber(CursorPositionFromTop));

        public void AdaptCodeForBufferSize(int maxLineLength)
            => _codeFormatter.AdaptCodeForBufferSize(maxLineLength);

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

        public void ReplaceLineAt(int lineNumber, string newLine)
        {
            CheckIfLineExistsAt(lineNumber);
            _sourceCode[_codeFormatter.LineNumberToIndex(lineNumber)] = _codeFormatter.ModifyLine(newLine, lineNumber);
        }

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

        public void AppendLine(string newPart)
        {
            if (IsUpperCaseSymbol(newPart))
                newPart = KeyToUpperCaseSymbol(newPart);
            else if (IsLowerCaseSymbol(newPart))
                newPart = newPart.ToLower();

            var lineNumber = _codeFormatter.IndexToLineNumber(CursorPositionFromTop);
            var newLine = _codeFormatter.SeparateLineFromLineNumber(
                _codeFormatter.GroupNewLineOfACurrentOne(newPart, CursorPositionFromTop, CursorPositionFromLeft));

            _sourceCode[_codeFormatter.LineNumberToIndex(lineNumber)] = newLine;
            SetCursorPositionFromLeftAt(CursorPositionFromLeft + newPart.Length);
        }

        public void Enter()
            => _keyboardCommandsProvider.Enter();

        public void Backspace()
            => _keyboardCommandsProvider.Backspace();


        #region Private Methods
        private void CheckIfLineExistsAt(int lineNumber)
        {
            if (_sourceCode.Count < lineNumber || lineNumber < 1)
            {
                throw new OutOfCodeBufferSizeException($"Line number {lineNumber} does not exist!");
            }
        }

        private bool IsUpperCaseSymbol(string key)
            => Regex.IsMatch(key, @"Shift\+[\w]", RegexOptions.IgnoreCase) || Console.CapsLock;

        private bool IsLowerCaseSymbol(string key)
            => Regex.IsMatch(key, "[A-Z]", RegexOptions.IgnoreCase) && !Console.CapsLock;

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
}