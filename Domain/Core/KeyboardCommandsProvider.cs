using System.Collections.Generic;
using Domain.Core.CodeFormatters;
using Domain.Core.Cursors;

namespace Domain.Core
{
    public class KeyboardCommandsProvider
    {
        private readonly List<string> _code;
        private readonly ICodeFormatter _formatter;
        private readonly ICursor _cursor;
        private int CursorPositionFromTop => _cursor.CursorPositionFromTop;
        private int CursorPositionFromLeft => _cursor.CursorPositionFromLeft;

        public KeyboardCommandsProvider(List<string> code, ICodeFormatter formatter, ICursor cursor)
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

            _code[CursorPositionFromTop] = _formatter.SeparateLineFromLineNumber(line[..CursorPositionFromLeft]);

            var newLine = line[CursorPositionFromLeft..];
            var targetIndex = CursorPositionFromTop + 1;
            newLine = _formatter.GetTabulationForLineAt(_formatter.IndexToLineNumber(targetIndex), newLine) + newLine;

            _code.Insert(targetIndex, newLine);
            _cursor.IncCursorPositionFromTop();
        }

        public void Backspace()
        {
            var lineNumber = _formatter.IndexToLineNumber(CursorPositionFromTop);
            var line = _formatter.GroupOutputLineAt(lineNumber)[..^1];
            var lineContent = _formatter.SeparateLineFromLineNumber(line);

            var isThisAFirstSymbol = line.Length - lineContent.Length == CursorPositionFromLeft;
            var isThisAFirstLine = CursorPositionFromTop == 0;
            var isLineEmpty = lineContent == "";
            var isThisALastLine = CursorPositionFromTop == _code.Count - 1;

            if (isThisAFirstSymbol && !isThisAFirstLine)
            {
                var previousLine = _formatter.GroupOutputLineAt(CursorPositionFromTop)[..^1];
                var previousLineContent = _formatter.GetLineAt(_formatter.IndexToLineNumber(CursorPositionFromTop));

                _cursor.DecCursorPositionFromTop();
                _code[CursorPositionFromTop] += lineContent;
                _code.RemoveAt(_formatter.IndexToLineNumber(CursorPositionFromTop));
                _cursor.SetCursorPositionFromLeftAt(previousLine.Length - (previousLineContent == "" ? 0 : 1));
            }
            else if ((!isLineEmpty && !isThisAFirstLine) || (isThisAFirstLine && !isThisAFirstSymbol))
            {
                _cursor.DecCursorPositionFromLeft();
                _code[CursorPositionFromTop] = _formatter.SeparateLineFromLineNumber(
                    line.Remove(CursorPositionFromLeft, 1));
            }
        }
    }
}