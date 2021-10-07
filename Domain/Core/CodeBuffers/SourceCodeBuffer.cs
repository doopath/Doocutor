using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Domain.Core.CodeBuffers.CodePointers;
using Domain.Core.CodeFormatters;
using Domain.Core.Cursors;
using Domain.Core.Exceptions;

namespace Domain.Core.CodeBuffers
{
    public class SourceCodeBuffer : ICodeBuffer
    {
        private static readonly List<string> SourceCode = InitialSourceCode.GetInitialSourceCode();
        private static readonly ICodeFormatter CodeFormatter = new SourceCodeFormatter(SourceCode);
        private static readonly ICursor Cursor = new CodeBufferCursor(
            SourceCode,
            CodeFormatter,
            InitialSourceCode.InitialCursorPositionFromLeft,
            InitialSourceCode.InitialCursorPositionFromTop);
        private static readonly KeyboardCommandsProvider KeyboardCommandsProvider =
            new(SourceCode, CodeFormatter, Cursor);

        public int BufferSize => SourceCode.Count;

        public int CursorPositionFromTop => Cursor.CursorPositionFromTop;

        public int CursorPositionFromLeft => Cursor.CursorPositionFromLeft;

        public string CurrentLine => GetCurrentLine();

        public string CurrentLinePrefix =>
            CodeFormatter.GetLineAt(CodeFormatter.IndexToLineNumber(CursorPositionFromTop)).Split(CurrentLine)[0];

        /// <summary>
        /// Get source code numerated by lines.
        /// For example then you do code in a text editor or and IDE you
        /// want to know numbers of lines for easier navigating.
        /// </summary>
        public string CodeWithLineNumbers => CodeFormatter.GetSourceCodeWithLineNumbers();

        /// <summary>
        /// Get bare code. Without line numbers and '\n' symbols.
        /// </summary>
        public string Code => CodeFormatter.GetSourceCode();

        /// <summary>
        /// Get bare code split by line ends.
        /// </summary>
        public string[] Lines => SourceCode.ToArray();

        public string GetLineAt(int lineNumber) => CodeFormatter.GetLineAt(lineNumber);

        public string[] GetCodeBlock(ICodeBlockPointer pointer)
            => Lines[CodeFormatter.LineNumberToIndex(pointer.StartLineNumber)..CodeFormatter.LineNumberToIndex(pointer.EndLineNumber)];

        public void RemoveCodeBlock(ICodeBlockPointer pointer)
        {
            CheckIfLineExistsAt(CodeFormatter.LineNumberToIndex(pointer.EndLineNumber - 1));

            for (var i = 0; i < pointer.EndLineNumber - pointer.StartLineNumber; i++)
            {
                SourceCode.RemoveAt(CodeFormatter.LineNumberToIndex(pointer.StartLineNumber));
            }

            AddLineIfBufferIsEmpty();
            SetPointerAtLastLineIfNecessary();
        }

        public void RemoveLineAt(int lineNumber)
        {
            CheckIfLineExistsAt(lineNumber);
            SourceCode.RemoveAt(CodeFormatter.LineNumberToIndex(lineNumber));
            SetPointerAtLastLineIfNecessary();
        }

        public void SetCursorPositionFromTopAt(int position)
            => Cursor.SetCursorPositionFromTopAt(position);

        public void SetCursorPositionFromLeftAt(int position)
            => Cursor.SetCursorPositionFromLeftAt(position);

        public void IncCursorPositionFromLeft()
            => Cursor.IncCursorPositionFromLeft();

        public void DecCursorPositionFromLeft()
            => Cursor.DecCursorPositionFromLeft();

        public void IncCursorPositionFromTop()
            => Cursor.IncCursorPositionFromTop();

        public void DecCursorPositionFromTop()
            => Cursor.DecCursorPositionFromTop();

        public void ReplaceLineAt(int lineNumber, string newLine)
        {
            CheckIfLineExistsAt(lineNumber);
            SourceCode[CodeFormatter.LineNumberToIndex(lineNumber)] = CodeFormatter.ModifyLine(newLine, lineNumber);
        }

        public void Write(string line)
        {
            SourceCode.Insert(
                Cursor.CursorPositionFromTop,
                CodeFormatter.ModifyLine(line, CodeFormatter.IndexToLineNumber(Cursor.CursorPositionFromTop)));
            Cursor.IncCursorPositionFromTop();
        }

        public void WriteAfter(int lineNumber, string line)
        {
            CheckIfLineExistsAt(lineNumber);
            SourceCode.Insert(lineNumber, CodeFormatter.ModifyLine(line, lineNumber));
        }

        public void WriteBefore(int lineNumber, string line)
        {
            if (CodeFormatter.LineNumberToIndex(lineNumber) > -1 && lineNumber <= SourceCode.Count)
                SourceCode.Insert(CodeFormatter.LineNumberToIndex(lineNumber), CodeFormatter.ModifyLine(line, 1));
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

            var lineNumber = CodeFormatter.IndexToLineNumber(CursorPositionFromTop);
            var newLine = CodeFormatter.SeparateLineFromLineNumber(
                CodeFormatter.GroupNewLineOfACurrentOne(newPart, CursorPositionFromTop, CursorPositionFromLeft));

            SourceCode[CodeFormatter.LineNumberToIndex(lineNumber)] = newLine;
            SetCursorPositionFromLeftAt(CursorPositionFromLeft + newPart.Length);
        }

        public void Enter()
            => KeyboardCommandsProvider.Enter();

        public void Backspace()
            => KeyboardCommandsProvider.Backspace();

        private void CheckIfLineExistsAt(int lineNumber)
        {
            if (SourceCode.Count < lineNumber || lineNumber < 1)
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
            => Cursor.CursorPositionFromTop = Cursor.CursorPositionFromTop <= SourceCode.Count
                ? Cursor.CursorPositionFromTop
                : SourceCode.Count;

        private void AddLineIfBufferIsEmpty()
        {
            if (SourceCode.Count == 0)
                SourceCode.Add("");
        }

        private string GetCurrentLine()
            => SourceCode.Count > 0 ? SourceCode[CursorPositionFromTop] : "";
    }

    internal static class InitialSourceCode
    {
        public static List<string> GetInitialSourceCode() => new(new[] {
            "namespace Doocutor",
            "{",
            "    public class Program",
            "    {",
            "        public static void Main(string[] args)",
            "        {",
            "        }",
            "    }",
            "}",
        });

        public const int InitialCursorPositionFromTop = 6;
        public const int InitialCursorPositionFromLeft = 14;
    }
}
