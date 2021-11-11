using System.Collections.Generic;
using Domain.Core.Cursors;
using Domain.Core.TextBufferFormatters;
using Libraries.Core;

namespace Domain.Core
{
    public class KeyboardCommandsProvider
    {
        private readonly List<string> _code;
        private readonly ITextBufferFormatter _formatter;
        private readonly ICursor _cursor;
        private int CursorPositionFromTop => _cursor.CursorPositionFromTop;
        private int CursorPositionFromLeft => _cursor.CursorPositionFromLeft;
        private int? MaxLineLength => _formatter.MaxLineLength;

        public KeyboardCommandsProvider(List<string> code, ITextBufferFormatter formatter, ICursor cursor)
        {
            _code = code;
            _formatter = formatter;
            _cursor = cursor;
        }

        public void Enter()
        {
            var lineNumber = _formatter.IndexToLineNumber(CursorPositionFromTop);
            var line = _formatter.GroupOutputLineAt(lineNumber)[..^1];
            var lineContent = _formatter.GetLineAt(lineNumber);

            _code[CursorPositionFromTop] = _formatter.SeparateLineFromLineNumber(
                    line[..CursorPositionFromLeft]);

            var newLine = line[CursorPositionFromLeft..];
            var targetIndex = CursorPositionFromTop + 1;
            var targetLineNumber = _formatter.IndexToLineNumber(targetIndex);

            _code.Insert(targetIndex, newLine);
            _cursor.IncCursorPositionFromTop();
            _cursor.SetCursorPositionFromLeftAt(_formatter.GetPrefixLength(targetLineNumber));
        }

        public void Backspace()
        {
            var lineNumber = _formatter.IndexToLineNumber(CursorPositionFromTop);
            var line = _formatter.GroupOutputLineAt(lineNumber)[..^1];
            var lineContent = _formatter.SeparateLineFromLineNumber(line);

            var isThisAFirstSymbol = line.Length - lineContent.Length == CursorPositionFromLeft;
            var isThisAFirstLine = CursorPositionFromTop == 0;
            var isLineEmpty = lineContent == "";

            if (isThisAFirstSymbol && !isThisAFirstLine)
            {
                int previousLineLength = _formatter.GroupOutputLineAt(CursorPositionFromTop)[..^1].Length;

                if (previousLineLength == MaxLineLength && !isLineEmpty)
                {
                    _cursor.DecCursorPositionFromLeft();
                    return;
                }

                _cursor.DecCursorPositionFromTop();

                _code[CursorPositionFromTop] += lineContent;
                _code.RemoveAt(_formatter.IndexToLineNumber(CursorPositionFromTop));

                var currentLine = _formatter.GroupOutputLineAt(CursorPositionFromTop + 1)[..^1];

                _cursor.SetCursorPositionFromLeftAt(currentLine.Length - lineContent.Length);
            }
            else if (!(isLineEmpty || isThisAFirstLine) || (isThisAFirstLine && !isThisAFirstSymbol))
            {
                _cursor.DecCursorPositionFromLeft();
                _code[CursorPositionFromTop] = _formatter.SeparateLineFromLineNumber(
                    line.Remove(CursorPositionFromLeft, 1));
            }
        }
    }
}
