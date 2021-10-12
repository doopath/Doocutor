using System.Collections.Generic;
using Domain.Core.CodeFormatters;
using Domain.Core.Cursors;
using NUnit.Framework;

namespace Tests.Core.Cursors
{
    [TestFixture]
    public class CodeBufferCursorTests
    {
        private ICursor _cursor;
        private List<string> _code;
        private ICodeFormatter _formatter;
        private int _initialPositionFromTop;
        private int _initialPositionFromLeft;

        [SetUp]
        public void Setup()
        {
            _code = new List<string>();
            _formatter = new SourceCodeFormatter(_code);
            _initialPositionFromTop = 0;
            _initialPositionFromLeft = 0;
            _cursor = new CodeBufferCursor(_code, _formatter, _initialPositionFromTop, _initialPositionFromLeft);

            FillCode();
        }

        [Test]
        public void SetCursorPositionFromLeftAtTheStartOfTheFirstLineTest()
        {
            var startOfTheFirstLine = _formatter.GetPrefixLength(1);

            _cursor.SetCursorPositionFromLeftAt(startOfTheFirstLine);

            var isTheCursorTopPositionCorrect = _cursor.CursorPositionFromTop == 0;
            var isTheCursorLeftPositionCorrect = _cursor.CursorPositionFromLeft == startOfTheFirstLine;

            Assert.True(isTheCursorTopPositionCorrect,
                $"Top position of the cursor isn't correct! ({_cursor.CursorPositionFromTop} != 0)");
            Assert.True(isTheCursorLeftPositionCorrect,
                $"Left position of the cursor isn't correct! ({_cursor.CursorPositionFromLeft} != {startOfTheFirstLine})");

        }

        private void FillCode()
        {
            _code.Add("----------");
            _code.Add("----------");
            _code.Add("\\");
            _code.Add("");
        }
    }
}