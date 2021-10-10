using System.Collections.Generic;
using Domain.Core;
using Domain.Core.CodeFormatters;
using Domain.Core.Cursors;
using NUnit.Framework;

namespace Tests.Core
{
    [TestFixture]
    public class KeyboardCommandsProviderTests
    {
        private KeyboardCommandsProvider _provider;
        private List<string> _code;
        private ICodeFormatter _formatter;
        private ICursor _cursor;
        private int _initialCursorPositionFromTop;
        private int _initialCursorPositionFromLeft;

        [SetUp]
        public void Setup()
        {
            _code = new List<string>();
            _formatter = new SourceCodeFormatter(_code);

            FillCode();

            _initialCursorPositionFromTop = 0;
            _initialCursorPositionFromLeft = _formatter.GetPrefixLength(1);
            _cursor = new CodeBufferCursor(_code, _formatter, _initialCursorPositionFromTop, _initialCursorPositionFromLeft);
            _provider = new KeyboardCommandsProvider(_code, _formatter, _cursor);
        }

        [TearDown]
        public void Teardown()
        {
            _code = null;
            _formatter = null;
            _cursor = null;
            _provider = null;
        }

        [Test]
        public void EnterAtTheBeginningOfTheLineTest()
        {
            var initialFirstLine = _code[0];

            _provider.Enter();

            var prefixLength = _formatter.GetPrefixLength(2);
            var isTheFirstLineEmpty = _code[0] == string.Empty;
            var isTheSecondLineThePreviousFirst = _code[1] == initialFirstLine;
            var isTheCursorPositionFromTopCorrect = _cursor.CursorPositionFromTop == 1;
            var isTheCursorPositionFromLeftCorrect = _cursor.CursorPositionFromLeft == prefixLength;

            Assert.True(isTheFirstLineEmpty, $"The first line isn't empty! ({_code[0]})");
            Assert.True(isTheSecondLineThePreviousFirst,
                $"The second line isn't the previous first! ({_code[1]} != {initialFirstLine})");
            Assert.True(isTheCursorPositionFromTopCorrect,
                $"Top position of the cursor is not correct! ({_cursor.CursorPositionFromTop} != 1)");
            Assert.True(isTheCursorPositionFromLeftCorrect,
                $"Left position of the cursor is not correct! ({_cursor.CursorPositionFromLeft} != {prefixLength})");
        }

        [Test]
        public void EnterAtTheMiddleOfTheLineTest()
        {
            var prefixLength = _formatter.GetPrefixLength(1);
            var initialFirstLine = _code[0];
            var middlePositionOfTheCurrentLine = initialFirstLine.Length / 2 + prefixLength;

            _cursor.SetCursorPositionFromLeftAt(middlePositionOfTheCurrentLine);
            _provider.Enter();

            var isTheFirstLineCorrect = _code[0] == initialFirstLine[..(middlePositionOfTheCurrentLine - prefixLength)];
            var isTheSecondLineCorrect = _code[1] == initialFirstLine[(middlePositionOfTheCurrentLine - prefixLength)..];
            var isTopPositionOfTheCursorCorrect = _cursor.CursorPositionFromTop == 1;
            var isLeftPositionOfTheCursorCorrect = _cursor.CursorPositionFromLeft == prefixLength;
            var isCodeLinesCountCorrect = _code.Count == 3;

            Assert.True(isTheFirstLineCorrect, "The first line isn't correct!");
            Assert.True(isTheSecondLineCorrect, "The second line isn't correct!");
            Assert.True(isTopPositionOfTheCursorCorrect,
                $"Top position of the cursor isn't correct ({_cursor.CursorPositionFromTop} != 1)");
            Assert.True(isLeftPositionOfTheCursorCorrect,
                $"Left position of the cursor isn't correct ({_cursor.CursorPositionFromLeft} != {prefixLength})");
            Assert.True(isCodeLinesCountCorrect, $"Code lines count isn't correct! ({_code.Count} != 3)");
        }

        [Test]
        public void EnterAtTheEndOfTheLineTest()
        {
            var prefixLength = _formatter.GetPrefixLength(1);
            var initialFirstLine = _code[0];

            _cursor.SetCursorPositionFromLeftAt(initialFirstLine.Length + prefixLength);
            _provider.Enter();

            var isTheFirstLineCorrect = _code[0] == initialFirstLine;
            var isTheSecondLineCorrect = _code[1] == string.Empty;
            var isTopPositionOfTheCursorCorrect = _cursor.CursorPositionFromTop == 1;
            var isLeftPositionOfTheCursorCorrect = _cursor.CursorPositionFromLeft == prefixLength;

            Assert.True(isTheFirstLineCorrect,
                $"The first line isn't correct! ({_code[0]} != {initialFirstLine})");
            Assert.True(isTheSecondLineCorrect,
                $"The second line isn't correct! ({_code[1]} != \"\")");
            Assert.True(isTopPositionOfTheCursorCorrect,
                $"Top position of the cursor isn't correct! ({_cursor.CursorPositionFromTop} != 1)");
            Assert.True(isLeftPositionOfTheCursorCorrect,
                $"Left position of the cursor isn't correct! ({_cursor.CursorPositionFromLeft} != {prefixLength})");

        }

        [Test]
        public void EnterAtTheEndOfTheLastLineTest()
        {
            var prefixLength = _formatter.GetPrefixLength(1);
            var initialSecondLine = _code[1];

            _cursor.SetCursorPositionFromTopAt(1);
            _cursor.SetCursorPositionFromLeftAt(initialSecondLine.Length + prefixLength);
            _provider.Enter();

            var isTheSecondLineCurrect = _code[1] == initialSecondLine;
            var isTheThirdLineCorrect = _code[2] == string.Empty;
            var isTheCursorPositionFromTopCorrect = _cursor.CursorPositionFromTop == 2;
            var isTheCursorPositionFromLeftCorrect = _cursor.CursorPositionFromLeft == prefixLength;

            Assert.True(isTheSecondLineCurrect,
                $"The second line isn't correct! ({_code[1]} != {initialSecondLine})");
            Assert.True(isTheThirdLineCorrect,
                $"The third line isn't correct! ({_code[2]} != \"\")");
            Assert.True(isTheCursorPositionFromTopCorrect,
                $"Top position of the cursor isn't correct! ({_cursor.CursorPositionFromTop} != 2)");
            Assert.True(isTheCursorPositionFromLeftCorrect,
                $"Left position of the cursor isn't correct! ({_cursor.CursorPositionFromLeft} != {prefixLength})");
        }

        private void FillCode()
        {
            _code.Add("--------------------------------");
            _code.Add("----------------");
        }
    }
}