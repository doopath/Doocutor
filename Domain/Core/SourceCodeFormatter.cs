using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Domain.Core.Exceptions;

namespace Domain.Core
{
    public sealed class SourceCodeFormatter
    {
        private readonly List<string> _sourceCode;

        public SourceCodeFormatter(List<string> sourceCode)
        {
            _sourceCode = sourceCode;
        }

        public string GroupNewLineOfACurrentOne(string newPart, int cursorPositionFromTop, int cursorPositionFromLeft)
        {
            var lineNumber = cursorPositionFromTop + 1;
            var currentLine = GroupOutputLineAt(lineNumber)[..^1];

            return currentLine[..cursorPositionFromLeft] + newPart + currentLine[cursorPositionFromLeft..];
        }

        public string SeparateLineFromLineNumber(string line)
            => string.Join("|", line.Split("|")[1..]);

        public string ModifyLine(string line, int lineNumber)
            => GetTabulationForLineAt(lineNumber, line) + line.Trim();

        public string GetTabulationForLineAt(int lineNumber, string line)
        {

            int previousTabulationLength = 0;
            int additionalTabulationLength = 0;

            if (lineNumber != 1)
            {
                var previousLine = _sourceCode[LineNumberToIndex(lineNumber) - 1];
                previousTabulationLength = previousLine.Length - previousLine.Trim().Length;
                additionalTabulationLength = LineHasOpeningBrace(previousLine) ? 4 : 0;
            }

            additionalTabulationLength -= LineHasClosingBrace(line) ? 4 : 0;

            return new string(' ', previousTabulationLength + additionalTabulationLength);
        }

        public bool LineHasOpeningBrace(string line)
        {
            RemoveAllButBracesIn(ref line);
            RemoveAllCoupleBracesIn(ref line);

            return IsOpeningBrace(line);
        }

        public bool LineHasClosingBrace(string line)
        {
            RemoveAllButBracesIn(ref line);
            RemoveAllCoupleBracesIn(ref line);

            return IsClosingBrace(line);
        }

        public bool IsClosingBrace(string line) => line.Equals("}");

        public bool IsOpeningBrace(string line) => line.Equals("{");

        public string RemoveAllButBracesIn(ref string line)
            => line = Regex.Replace(line, @"[^{}]", string.Empty);

        public void RemoveAllCoupleBracesIn(ref string line)
        {
            while (LineContainsBraces(line))
                line = RemoveCoupleBracesIn(ref line);
        }

        public string RemoveCoupleBracesIn(ref string line)
            => line = line.Replace(@"{}", string.Empty);

        public bool LineContainsBraces(string line)
            => line.Contains("{") && line.Contains(("}"));
        public string GetLineAt(int lineNumber)
        {
            CheckIfLineExistsAt(lineNumber);
            return _sourceCode.ToArray()[LineNumberToIndex(lineNumber)];
        }

        public string GetSourceCodeWithLineNumbers()
            => string.Join("", _sourceCode.Select((_, i) => GroupOutputLineAt(IndexToLineNumber(i))).ToArray());

        public string GroupOutputLineAt(int lineNumber)
            => $"  {lineNumber}{GetOutputSpacesForLineAt(lineNumber)}|{_sourceCode[LineNumberToIndex(lineNumber)]}\n";

        public string GetOutputSpacesForLineAt(int lineNumber)
            => ' ' + new string(' ', GetTimesOfSpacesRepeationForLineAt(lineNumber));

        public int GetTimesOfSpacesRepeationForLineAt(int lineNumber)
            => _sourceCode.Count.ToString().Length - lineNumber.ToString().Length;

        public string GetSourceCode() => string.Join("", _sourceCode.Select(l => l + "\n"));

        public int IndexToLineNumber(int index) => index + 1;

        public int LineNumberToIndex(int lineNumber) => lineNumber - 1;

        private void CheckIfLineExistsAt(int lineNumber)
        {
            if (_sourceCode.Count < lineNumber || lineNumber < 1)
            {
                throw new OutOfCodeBufferSizeException($"Line number {lineNumber} does not exist!");
            }
        }
    }
}
