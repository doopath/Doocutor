using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Domain.Core.CodeBuffers.CodePointers;
using Domain.Core.Exceptions;

namespace Domain.Core.CodeBuffers
{
    public class SourceCodeBuffer : ICodeBuffer
    {
        private static readonly List<string> SourceCode = InitialSourceCode.GetInitialSourceCode();
        private static readonly SourceCodeFormatter CodeFormatter = new(SourceCode);
        private static readonly CodeBufferCursor Cursor = new(
            SourceCode,
            CodeFormatter,
            InitialSourceCode.InitialCursorPositionFromLeft,
            InitialSourceCode.InitialCursorPositionFromTop);

        /// <summary>
        /// Count of lines in the SourceCodeBuffer.
        /// </summary>
        public int BufferSize => SourceCode.Count;

        /// <summary>
        /// Presents an offset or a count of "\n" symbols from top side.
        /// </summary>
        public int CursorPositionFromTop => Cursor.CursorPositionFromTop;

        /// <summary>
        /// Presents an offset or a count of whitespaces from left side.
        /// </summary>
        public int CursorPositionFromLeft => Cursor.CursorPositionFromLeft;

        /// <summary>
        /// A current line of the buffer.
        /// </summary>
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
        /// Get pure code. Without line numbers and other stuff.
        /// </summary>
        public string Code => CodeFormatter.GetSourceCode();

        /// <summary>
        /// Get pure code split by line breaks.
        /// </summary>
        public string[] Lines => SourceCode.ToArray();

        public string GetLineAt(int lineNumber) => CodeFormatter.GetLineAt(lineNumber);

        public string[] GetCodeBlock(ICodeBlockPointer pointer)
            => Lines[CodeFormatter.LineNumberToIndex(pointer.StartLineNumber)..CodeFormatter.LineNumberToIndex(pointer.EndLineNumber)];

        /// <summary>
        /// Remove a few lines from s to e. Please, pay attention that the last
        /// line in an interval will not be deleted. So, if you set [1, 2, 3, 4] lines
        /// then only 1st, 2nd and 3rd lines will be deleted.
        /// </summary>
        /// <param name="pointer">A pointer that indicate the interval of lines.</param>
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

            var lineNumber = CursorPositionFromTop + 1;
            var newLine = CodeFormatter.SeparateLineFromLineNumber(
                CodeFormatter.GroupNewLineOfACurrentOne(newPart, CursorPositionFromTop, CursorPositionFromLeft));

            SourceCode[CodeFormatter.LineNumberToIndex(lineNumber)] = newLine;
            SetCursorPositionFromLeftAt(CursorPositionFromLeft + newPart.Length);
        }

        public void Enter()
        {
            var lineNumber = CodeFormatter.IndexToLineNumber(CursorPositionFromTop);
            var line = CodeFormatter.GroupOutputLineAt(lineNumber)[..^1];

            SourceCode[CursorPositionFromTop] = CodeFormatter.SeparateLineFromLineNumber(line[..CursorPositionFromLeft]);

            var newLine = line[CursorPositionFromLeft..];
            var targetIndex = CursorPositionFromTop + 1;
            newLine = CodeFormatter.GetTabulationForLineAt(CodeFormatter.IndexToLineNumber(targetIndex), newLine) + newLine;

            SourceCode.Insert(targetIndex, newLine);
            Cursor.IncCursorPositionFromTop();
        }

        public void Backspace()
        {
            var lineNumber = CodeFormatter.IndexToLineNumber(CursorPositionFromTop);
            var line = CodeFormatter.GroupOutputLineAt(lineNumber)[..^1];

            if (line.Length - CodeFormatter.SeparateLineFromLineNumber(line).Length == CursorPositionFromLeft)
            {
                var previousLine = CodeFormatter.GroupOutputLineAt(CursorPositionFromTop)[..^1];

                SourceCode.RemoveAt(CursorPositionFromTop);
                DecCursorPositionFromTop();
                SetCursorPositionFromLeftAt(previousLine.Length);
            }
            else
            {
                SourceCode[CursorPositionFromTop] = CodeFormatter.SeparateLineFromLineNumber(
                    line.Remove(CursorPositionFromLeft - 1, 1));
                DecCursorPositionFromLeft();
            }
        }

        private void CheckIfLineExistsAt(int lineNumber)
        {
            if (SourceCode.Count < lineNumber || lineNumber < 1)
            {
                throw new OutOfCodeBufferSizeException($"Line number {lineNumber} does not exist!");
            }
        }

        private bool IsOneSymbol(string key)
            => IsUpperCaseSymbol(key) || IsLowerCaseSymbol(key);

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
            ""
        });

        public const int InitialCursorPositionFromTop = 6;
        public const int InitialCursorPositionFromLeft = 14;
    }
}
