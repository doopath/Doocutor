using System.Collections.Generic;
using Domain.Core.CodeFormatters;
using Domain.Core.Cursors;
using Domain.Core.Exceptions;
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

        [Test]
        public void SetCursorPositionAtTheEndOfALineTest()
        {
            var lineLength = _formatter.GroupOutputLineAt(1)[..^1].Length;

            _cursor.SetCursorPositionFromLeftAt(lineLength);

            var isTheCursorTopPositionCorrect = _cursor.CursorPositionFromTop == 0;
            var isTheCursorLeftPositionCorrect = _cursor.CursorPositionFromLeft == lineLength;

            Assert.True(isTheCursorTopPositionCorrect,
                $"Top position of the cursor isn't correct! ({_cursor.CursorPositionFromTop} != 0)");
            Assert.True(isTheCursorLeftPositionCorrect,
                $"Left position of the cursor isn't correct! ({_cursor.CursorPositionFromLeft} != {lineLength})");
        }

        [Test]
        public void SetCursorPositionFromLeftAtAnOverflowedPositionTest()
        {
            var lineLength = _formatter.GroupOutputLineAt(1)[..^1].Length;

            _cursor.SetCursorPositionFromLeftAt(lineLength + 1);

            var targetPositionFromLeft = _formatter.GetPrefixLength(2);

            var isTheCursorTopPositionCorrect = _cursor.CursorPositionFromTop == 1;
            var isTheCursorLeftPositionCorrect = _cursor.CursorPositionFromLeft == targetPositionFromLeft;

            Assert.True(isTheCursorTopPositionCorrect,
                $"Top position of the cursor isn't correct! ({_cursor.CursorPositionFromTop} != 0)");
            Assert.True(isTheCursorLeftPositionCorrect,
                $"Left position of the cursor isn't correct! ({_cursor.CursorPositionFromLeft} != {targetPositionFromLeft})");
        }

        [Test]
        public void SetCursorPositionFromTopAtZeroLineTest()
        {
            try
            {
                _cursor.SetCursorPositionFromTopAt(-1);
            }
            catch (OutOfCodeBufferSizeException)
            {
                var isTheCursorPositionFromTopCorrect = _cursor.CursorPositionFromTop == 0;
                var isTheCursorPositionFromLeftCorrect = _cursor.CursorPositionFromLeft == _initialPositionFromLeft;

                Assert.True(isTheCursorPositionFromTopCorrect,
                    $"Top position of the cursor isn't correct! ({_cursor.CursorPositionFromTop} != 0)");
                Assert.True(isTheCursorPositionFromLeftCorrect,
                    $"Left position of the cursor isn't correct! ({_cursor.CursorPositionFromLeft} != {_initialPositionFromLeft})");

                return;
            }

            throw new AssertionException(
                "The SetCursorPositionFromTopAt method with an incorrect value should throw the OutOfCodeBufferSizeException!");
        }

        [Test]
        public void SetCursorPositionFromTopAtAnOverflowedPositionTest()
        {
            try
            {
                // This is an incorrect position, because of the
                // _code.Count value is greater than the index of the last line.
                _cursor.SetCursorPositionFromTopAt(_code.Count);
            }
            catch (OutOfCodeBufferSizeException)
            {
                var isTheCursorPositionFromTopCorrect = _cursor.CursorPositionFromTop == 0;
                var isTheCursorPositionFromLeftCorrect = _cursor.CursorPositionFromLeft == _initialPositionFromLeft;

                Assert.True(isTheCursorPositionFromTopCorrect,
                    $"Top position of the cursor isn't correct! ({_cursor.CursorPositionFromTop} != 0)");
                Assert.True(isTheCursorPositionFromLeftCorrect,
                    $"Left position of the cursor isn't correct! ({_cursor.CursorPositionFromLeft} != {_initialPositionFromLeft})");

                return;
            }

            throw new AssertionException(
                "The SetCursorPositionFromTopAt method with an incorrect value should throw the OutOfCodeBufferSizeException!");
        }

        [Test]
        public void SetCursorPositionAtTheNextLineWhichIsShorterTest()
        {
            var lineNumberOfTheSecondLine = 2;
            var positionOfTheSecondLine = lineNumberOfTheSecondLine - 1;
            var positionOfTheLastSymbolInTheSecondLine = _formatter
                .GroupOutputLineAt(lineNumberOfTheSecondLine)[..^1]
                .Length;

            _cursor.SetCursorPositionFromTopAt(positionOfTheSecondLine);
            _cursor.SetCursorPositionFromLeftAt(positionOfTheLastSymbolInTheSecondLine);
            _cursor.SetCursorPositionFromTopAt(positionOfTheSecondLine + 1);

            var positionOfTheThirdLine = positionOfTheSecondLine + 1;
            var thirdLine = _formatter.GroupOutputLineAt(3)[..^1];

            var isTheCursorPositionFromTopCorrect = _cursor.CursorPositionFromTop == positionOfTheThirdLine;
            var isTheCursorPositionFromLeftCorrect = _cursor.CursorPositionFromLeft == thirdLine.Length;

            Assert.True(isTheCursorPositionFromTopCorrect,
                $"Top position of the cursor isn't correct! ({_cursor.CursorPositionFromTop} != {positionOfTheThirdLine})");
            Assert.True(isTheCursorPositionFromLeftCorrect,
                $"Left position of the cursor isn't correct! ({_cursor.CursorPositionFromLeft} != {thirdLine.Length})");
        }

        [Test]
        public void SetCursorPositionAtTheMiddleOfTheNextLineTest()
        {
            var middlePositionOfTheFirstLine = _formatter.GroupOutputLineAt(1)[..^1].Length / 2;
            var nextPositionFromTop = 1;

            _cursor.SetCursorPositionFromLeftAt(middlePositionOfTheFirstLine);
            _cursor.SetCursorPositionFromTopAt(nextPositionFromTop);

            var isTheCursorPositionFromTopCorrect = _cursor.CursorPositionFromTop == nextPositionFromTop;
            var isTheCursorPositionFromLeftCorrect = _cursor.CursorPositionFromLeft == middlePositionOfTheFirstLine;

            Assert.True(isTheCursorPositionFromTopCorrect,
                $"Top position of the cursor isn't correct! ({_cursor.CursorPositionFromTop} != {nextPositionFromTop})");
            Assert.True(isTheCursorPositionFromLeftCorrect,
                $"Left position of the cursor isn't correct! ({_cursor.CursorPositionFromLeft} != {middlePositionOfTheFirstLine})");
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