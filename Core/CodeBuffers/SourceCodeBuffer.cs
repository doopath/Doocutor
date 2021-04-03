using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Doocutor.Core.CodeBuffers.CodePointers;
using Doocutor.Core.Exceptions;

namespace Doocutor.Core.CodeBuffers
{
    class SourceCodeBuffer : ICodeBuffer
    {
        private static readonly List<string> _sourceCode = InitialSourceCode.GetInitialSourceCode();
        private static int _pointerPosition = InitialSourceCode.GetInitialPointerPosition();

        public int BufferSize { get => _sourceCode.Count; }
        public string Code { get => GetSourceCode(); }

        public void RemoveCodeBlock(ICodeBlockPointer pointer)
        {
            CheckIfLineExistsAt(pointer.EndLineNumber);

            for (var i = 0; i < pointer.StartLineNumber - pointer.EndLineNumber + 1; i++)
            {
                _sourceCode.RemoveAt(pointer.StartLineNumber - 1);
            }

            SetPointerAtLastLineIfNecessary();
        }

        public void RemoveLine(int lineNumber)
        {
            CheckIfLineExistsAt(lineNumber);
            _sourceCode.RemoveAt(lineNumber - 1);
            SetPointerAtLastLineIfNecessary();
        }

        public void ReplaceLineAt(int lineNumber, string newLine)
        {
            CheckIfLineExistsAt(lineNumber);
            _sourceCode[lineNumber - 1] = ModifyLine(newLine, lineNumber);
        }

        public void Write(string line)
        {
            _sourceCode.Insert(_pointerPosition, ModifyLine(line));
            _pointerPosition++;
        }

        public void WriteAfter(int lineNumber, string line)
        {
            CheckIfLineExistsAt(lineNumber);
            _sourceCode.Insert(lineNumber, ModifyLine(line, lineNumber));
        }

        private void CheckIfLineExistsAt(int lineNumber)
        {
            if (_sourceCode.Count < lineNumber)
            {
                throw new OutOfCodeBufferSizeException($"Line number {lineNumber} does not exist!");
            }
        }

        private void SetPointerAtLastLineIfNecessary()
            => _pointerPosition = (_pointerPosition <= _sourceCode.Count) ? _pointerPosition : _sourceCode.Count;

        private string ModifyLine(string line, int lineNumber) => GetTabulationForLineAt(lineNumber) + line.Trim();
        private string ModifyLine(string line) => GetTabulationForLineAt(_pointerPosition) + line.Trim();

        private string GetTabulationForLineAt(int lineNumber)
        {
            int previousTabulationLength = _sourceCode[lineNumber - 1].Length - _sourceCode[lineNumber - 1].Trim().Length;
            int additionalTabs = (LineHasOpeningBracket(_sourceCode[lineNumber - 1])) ? 4 : 0;

            return new string(' ', previousTabulationLength + additionalTabs);
        }

        private bool LineHasOpeningBracket(string line)
        {
            line = Regex.Replace(line, @"[^{}]", string.Empty);

            while (line.Contains("{") && line.Contains("}"))
                line = line.Replace(@"\{\}", string.Empty);

            return line.Length == 1 && line.Equals("{");
        }

        private string GetOutputSpacesForLineAt(int lineNumber)
            => ' '.ToString() + new string(' ', _sourceCode.Count.ToString().Length - (lineNumber).ToString().Length);

        private string GroupOutputLineAt(int lineNumber)
            => $"  {lineNumber}{GetOutputSpacesForLineAt(lineNumber)}|{_sourceCode[lineNumber-1]}\n";

        private string GetSourceCode()
            => string.Join("", _sourceCode.Select((line, index) => GroupOutputLineAt(index + 1)).ToArray());
    }

    internal class InitialSourceCode
    {
        public static List<string> GetInitialSourceCode() => new(new string[] {
            "namespace Doocutor",
            "{",
            "   internal class Program",
            "   {",
            "       public static void Main(string[] args)",
            "       {",
            "       }",
            "   }",
            "}"
        });

        public static int GetInitialPointerPosition() => 6;
    }
}
