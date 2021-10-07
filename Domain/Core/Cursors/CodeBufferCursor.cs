using System.Collections.Generic;
using Domain.Core.CodeFormatters;
using Domain.Core.Exceptions;

namespace Domain.Core.Cursors
{
    public sealed class CodeBufferCursor : ICursor
    {
        private readonly List<string> _sourceCode;
        private readonly ICodeFormatter _formatter;
        public int CursorPositionFromLeft { get; set; }
        public int CursorPositionFromTop { get; set; }

        public CodeBufferCursor(
            List<string> sourceCode,
            ICodeFormatter formatter,
            int initialPositionFromLeft,
            int initialPositionFromTop)
        {
            _sourceCode = sourceCode;
            _formatter = formatter;
            CursorPositionFromLeft = initialPositionFromLeft;
            CursorPositionFromTop = initialPositionFromTop;
        }

        public void SetCursorPositionFromTopAt(int position)
        {
            CheckIfLineExistsAt(position + 1);

            var targetLineNumber = position + 1;
            var currentLineNumber = CursorPositionFromTop + 1;

            var currentLine = _formatter.GroupOutputLineAt(currentLineNumber);
            var currentLineContent = _formatter.GetLineAt(currentLineNumber);
            var currentLinePrefix = currentLine.Split(currentLineContent)[0];

            var targetLine = _formatter.GroupOutputLineAt(targetLineNumber);
            var targetLineContent = _formatter.GetLineAt(targetLineNumber);
            var targetLinePrefix = targetLine.Split(targetLineContent)[0];

            CursorPositionFromTop = position;

            if (CursorPositionFromLeft == currentLinePrefix.Length - 1 && targetLineContent != "")
                CursorPositionFromLeft = targetLinePrefix.Length;
            else if (targetLineContent == "")
                CursorPositionFromLeft = targetLinePrefix.Length - 1;
            else if (CursorPositionFromLeft >= targetLine.Length)
                CursorPositionFromLeft = targetLine.Length - 1;
        }

        public void SetCursorPositionFromLeftAt(int position)
        {
            var currentLineNumber = CursorPositionFromTop + 1;
            var currentLine = _formatter.GroupOutputLineAt(currentLineNumber);
            var currentLineContent = _formatter.GetLineAt(currentLineNumber);
            var currentLinePrefix = currentLine.Split(currentLineContent)[0];

            if (LineLengthOverflowedByCursor(position, currentLine) && NextLineOfTheBufferExists())
            {
                var nextLineNumber = currentLineNumber + 1;
                var nextLine = _formatter.GroupOutputLineAt(nextLineNumber);

                if (nextLine.Length < currentLine.Length)
                    CursorPositionFromLeft = nextLine.Length - 1;
                else
                    CursorPositionFromLeft = currentLine.Length - 1;

                CursorPositionFromTop++;
            }
            else if (LineLengthOverflowedByCursor(position, currentLine) && IsThisTheLastLine())
            {
                return;
            }
            else if (IsThisAFirstSymbolInALine(position, currentLinePrefix) && IsThisNotAFirstLine())
            {
                var previousLine = _formatter.GroupOutputLineAt(CursorPositionFromTop);

                CursorPositionFromTop--;
                CursorPositionFromLeft = previousLine.Length - 1;
            }
            else if (IsThisAFirstSymbolInALine(position, currentLinePrefix) && IsThisAFirstLine())
            {
                return;
            }
            else
            {
                CursorPositionFromLeft = position;
            }
        }


        #region Change position of the cursor by 1

        public void IncCursorPositionFromLeft()
            => SetCursorPositionFromLeftAt(CursorPositionFromLeft + 1);

        public void DecCursorPositionFromLeft()
            => SetCursorPositionFromLeftAt(CursorPositionFromLeft - 1);

        public void IncCursorPositionFromTop()
            => SetCursorPositionFromTopAt(CursorPositionFromTop + 1);

        public void DecCursorPositionFromTop()
            => SetCursorPositionFromTopAt(CursorPositionFromTop - 1);

        #endregion

        private void CheckIfLineExistsAt(int lineNumber)
        {
            if (_sourceCode.Count < lineNumber || lineNumber < 1)
                throw new OutOfCodeBufferSizeException($"Line number {lineNumber} does not exist!");
        }


        #region Condition simplifications

        private bool LineLengthOverflowedByCursor(int position, string currentLine)
            => position > currentLine.Length - 1;

        private bool NextLineOfTheBufferExists()
            => CursorPositionFromTop < _sourceCode.Count - 1;

        private bool IsThisTheLastLine()
            => CursorPositionFromTop == _sourceCode.Count - 1;

        private bool IsThisAFirstSymbolInALine(int position, string currentLinePrefix)
            => position <= currentLinePrefix.Length - 1;

        private bool IsThisNotAFirstLine()
            => CursorPositionFromTop > 0;

        private bool IsThisAFirstLine()
            => CursorPositionFromTop == 0;

        #endregion
    }
}
