using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Doocutor.Core.CodeBuffers.CodePointers;
using Doocutor.Core.Exceptions;

namespace Doocutor.Core.CodeBuffers
{
    internal class SourceCodeBuffer : ICodeBuffer
    {
        private static readonly List<string> SourceCode = InitialSourceCode.GetInitialSourceCode();
        private static int _pointerPosition = InitialSourceCode.InitialPointerPosition;

        /// <summary>
        /// Count of lines in the SourceCodeBuffer.
        /// </summary>
        public int BufferSize => SourceCode.Count;
        
        /// <summary>
        /// SourceCodeBuffer has a pointer that's current line number "to change".
        /// That means if you add a line of code (EditorCommand) then that will be
        /// added at line = pointer position.
        /// </summary>
        public int CurrentPointerPosition => _pointerPosition;

        /// <summary>
        /// Get source code numerated by lines.
        /// For example then you do code in a text editor or and IDE you
        /// want to know numbers of lines for easier navigating.
        /// </summary>
        public string CodeWithLineNumbers => GetSourceCodeWithLineNumbers();

        /// <summary>
        /// Get pure code. Without line numbers and other stuff.
        /// </summary>
        public string Code => GetSourceCode();

        /// <summary>
        /// Get pure code split by line breaks.
        /// </summary>
        public string[] Lines => SourceCode.ToArray();

        public string GetLineAt(int lineNumber)
        {
            CheckIfLineExistsAt(lineNumber);
            return Lines[LineNumberToIndex(lineNumber)];   
        }

        public string[] GetCodeBlock(ICodeBlockPointer pointer)
            => Lines[LineNumberToIndex(pointer.StartLineNumber)..LineNumberToIndex(pointer.EndLineNumber)];

        /// <summary>
        /// Remove a few lines from s to e. Please, pay attention that the last
        /// line in an interval will not be deleted. So, if you set [1, 2, 3, 4] lines
        /// then only 1st, 2nd and 3rd lines will be deleted.
        /// </summary>
        /// <param name="pointer">A pointer that indicate the interval of lines.</param>
        public void RemoveCodeBlock(ICodeBlockPointer pointer)
        {
            CheckIfLineExistsAt(LineNumberToIndex(pointer.EndLineNumber - 1));
            
            for (var i = 0; i < pointer.EndLineNumber - pointer.StartLineNumber; i++)
            {
                SourceCode.RemoveAt(LineNumberToIndex(pointer.StartLineNumber));
            }

            AddLineIfBufferIsEmpty();
            SetPointerAtLastLineIfNecessary();
        }

        public void RemoveLineAt(int lineNumber)
        {
            CheckIfLineExistsAt(lineNumber);
            SourceCode.RemoveAt(LineNumberToIndex(lineNumber));
            SetPointerAtLastLineIfNecessary();
        }

        /// <summary>
        /// Set current pointer position (see SourceCodeBuffer.CurrentPointerPosition) at a target line.
        /// </summary>
        /// <param name="lineNumber">Target line number.</param>
        public void SetPointerPositionAt(int lineNumber)
        {
            CheckIfLineExistsAt(lineNumber);
            _pointerPosition = lineNumber;
        }

        public void ReplaceLineAt(int lineNumber, string newLine)
        {
            CheckIfLineExistsAt(lineNumber);
            SourceCode[LineNumberToIndex(lineNumber)] = ModifyLine(newLine, lineNumber);
        }

        /// <summary>
        /// Write a line after current pointer position (see SourceCodeBuffer.CurrentPointerPosition).
        /// </summary>
        /// <param name="line">A new line.</param>
        public void Write(string line)
        {
            SourceCode.Insert(_pointerPosition, ModifyLine(line));
            _pointerPosition++;
        }

        public void WriteAfter(int lineNumber, string line)
        {
            CheckIfLineExistsAt(lineNumber);
            SourceCode.Insert(lineNumber, ModifyLine(line, lineNumber));
        }

        public void WriteBefore(int lineNumber, string line)
        {
            if (LineNumberToIndex(lineNumber) > -1 && lineNumber <= SourceCode.Count)
                SourceCode.Insert(LineNumberToIndex(lineNumber), ModifyLine(line, 1));
            else
                throw new OutOfCodeBufferSizeException(
           $"You cannot write line before the line with line number = {lineNumber}!");
        }

        private void CheckIfLineExistsAt(int lineNumber)
        {
            if (SourceCode.Count < lineNumber || lineNumber < 1)
            {
                throw new OutOfCodeBufferSizeException($"Line number {lineNumber} does not exist!");
            }
        }

        private void SetPointerAtLastLineIfNecessary()
            => _pointerPosition = (_pointerPosition <= SourceCode.Count) ? _pointerPosition : SourceCode.Count;

        private string ModifyLine(string line, int lineNumber) => GetTabulationForLineAt(lineNumber, line) + line.Trim();
        private string ModifyLine(string line) => GetTabulationForLineAt(_pointerPosition, line) + line.Trim();

        private string GetTabulationForLineAt(int lineNumber, string line)
        {
            int previousTabulationLength =
                SourceCode[LineNumberToIndex(lineNumber)]
                    .Length - SourceCode[LineNumberToIndex(lineNumber)].Trim().Length;
            int additionalTabs = LineHasOpeningBracket(SourceCode[LineNumberToIndex(lineNumber)]) ? 4 : 0;
            
            additionalTabs -= LineHasClosingBracket(line) ? 4 : 0;

            return new string(' ', previousTabulationLength + additionalTabs);
        }

        private bool LineHasOpeningBracket(string line)
        {
            line = RemoveAllButBracketsIn(line);
            RemoveAllCoupleBracketsIn(ref line);
            
            return line.Length == 1 && line.Equals("{");
        }

        private bool LineHasClosingBracket(string line)
        {
            line = RemoveAllButBracketsIn(line);
            RemoveAllCoupleBracketsIn(ref line);
            
            return line.Length == 1 && line.Equals("}");
        }
        
        private string RemoveAllButBracketsIn(string line)
            => Regex.Replace(line, @"[^{}]", string.Empty);

        private void RemoveAllCoupleBracketsIn(ref string line)
        {
            while (line.Contains("{") && line.Contains("}"))
                line = line.Replace(@"{}", string.Empty);
        }

        private string GetOutputSpacesForLineAt(int lineNumber)
            => ' ' + new string(' ', SourceCode.Count.ToString().Length - lineNumber.ToString().Length);

        private string GroupOutputLineAt(int lineNumber)
            => $"  {lineNumber}{GetOutputSpacesForLineAt(lineNumber)}|{SourceCode[lineNumber-1]}\n";

        private string GetSourceCodeWithLineNumbers()
            => string.Join("", SourceCode.Select((_, i) => GroupOutputLineAt(i + 1)).ToArray());

        private string GetSourceCode() => string.Join("", SourceCode.Select(l => l + "\n"));

        private void AddLineIfBufferIsEmpty()
        {
            if (SourceCode.Count == 0)
                SourceCode.Add("");
        }

        private int IndexToLineNumber(int index) => index + 1;

        private int LineNumberToIndex(int lineNumber) => lineNumber - 1;
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
            "}"
        });

        public const int InitialPointerPosition = 6;
    }
}
