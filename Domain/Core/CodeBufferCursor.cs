using System.Collections.Generic;
using Domain.Core.Exceptions;

namespace Domain.Core
{
    public sealed class CodeBufferCursor
    {
        private readonly List<string> _sourceCode;
        private readonly SourceCodeFormatter _formatter;
        public int CursorPositionFromLeft { get; set; }
        public int CursorPositionFromTop { get; set; }

        public CodeBufferCursor(
            List<string> sourceCode,
            SourceCodeFormatter formatter,
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

            var currentLine = _formatter.GroupOutputLineAt(CursorPositionFromTop + 1);
            var currentLineContent = _formatter.GetLineAt(CursorPositionFromTop + 1);
            var currentLinePrefix = currentLine.Split(currentLineContent)[0];

            var targetLine = _formatter.GroupOutputLineAt(position + 1);
            var targetLineContent = _formatter.GetLineAt(position + 1);
            var targetLinePrefix = targetLine.Split(targetLineContent)[0];

            if (CursorPositionFromLeft >= targetLine.Length)
                SetCursorPositionFromLeftAt(targetLine.Length - 1);
            else if (CursorPositionFromLeft == currentLinePrefix.Length - 1)
                SetCursorPositionFromLeftAt(targetLinePrefix.Length - 1);

            CursorPositionFromTop = position;
        }

        public void SetCursorPositionFromLeftAt(int position)
        {

            var currentLine = _formatter.GroupOutputLineAt(CursorPositionFromTop + 1);
            var currentLineContent = _formatter.GetLineAt(CursorPositionFromTop + 1);
            var currentLinePrefix = currentLine.Split(currentLineContent)[0];

            if (position > currentLine.Length - 1 && CursorPositionFromTop < _sourceCode.Count - 1)
            {
                var nextLine = _formatter.GroupOutputLineAt(CursorPositionFromTop + 2);

                IncCursorPositionFromTop();

                if (nextLine.Length < currentLine.Length)
                    SetCursorPositionFromLeftAt(nextLine.Length - 1);
                else
                    SetCursorPositionFromLeftAt(currentLine.Length);
            }
            else if (position > currentLine.Length - 1 && CursorPositionFromTop == _sourceCode.Count - 1)
            {
                return;
            }
            else if (position <= currentLinePrefix.Length - 1 && CursorPositionFromTop > 0)
            {
                var previousLine = _formatter.GroupOutputLineAt(CursorPositionFromTop);

                DecCursorPositionFromTop();
                SetCursorPositionFromLeftAt(previousLine.Length - 1);
            }
            else if (position <= currentLinePrefix.Length - 1 && CursorPositionFromTop == 0)
            {
                return;
            }
            else
            {
                CursorPositionFromLeft = position;
            }
        }

        public void IncCursorPositionFromLeft()
            => SetCursorPositionFromLeftAt(CursorPositionFromLeft + 1);

        public void DecCursorPositionFromLeft()
            => SetCursorPositionFromLeftAt(CursorPositionFromLeft - 1);

        public void IncCursorPositionFromTop()
            => SetCursorPositionFromTopAt(CursorPositionFromTop + 1);

        public void DecCursorPositionFromTop()
            => SetCursorPositionFromTopAt(CursorPositionFromTop - 1);

        private void CheckIfLineExistsAt(int lineNumber)
        {
            if (_sourceCode.Count < lineNumber || lineNumber < 1)
            {
                throw new OutOfCodeBufferSizeException($"Line number {lineNumber} does not exist!");
            }
        }
    }
}
