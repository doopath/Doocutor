using Domain.Core.CodeBufferContents;
using Domain.Core.CodeBuffers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Core.CodeBuffers
{
    [TestFixture]
    internal class SourceCodeBufferTests
    {
        private ICodeBuffer _codeBuffer;
        private ICodeBufferContent _codeBufferContent;

        [SetUp]
        public void Setup()
        {
            _codeBufferContent = new MockSourceCodeBufferContent();
            _codeBuffer = new SourceCodeBuffer(_codeBufferContent);
        }

        [Test]
        public void SizeTest()
        {
            var initialBufferSize = _codeBufferContent.SourceCode.Count;
            var extendedBufferSize = initialBufferSize + 1;

            var isSizeOfTheBufferCorrect = _codeBuffer.Size == initialBufferSize;

            Assert.True(isSizeOfTheBufferCorrect,
                $"Size of the code buffer isn't correct! ({_codeBuffer} != {initialBufferSize})");

            _codeBuffer.Write(string.Empty);

            isSizeOfTheBufferCorrect = _codeBuffer.Size == extendedBufferSize;

            Assert.True(isSizeOfTheBufferCorrect,
                $"Size of the code buffer isn't correct! ({_codeBuffer} != {initialBufferSize})");
        }

        [Test]
        public void CursorPositionFromTopTest()
        {
            var initialCursorPositionFromTop = _codeBufferContent.CursorPositionFromTop;
            var increasedCursorPositionFromTop = initialCursorPositionFromTop + 1;

            var isCursorPositionFromTopCorrect = _codeBuffer.CursorPositionFromTop == initialCursorPositionFromTop;

            Assert.True(isCursorPositionFromTopCorrect,
                "Cursor position from top isn't correct! " +
                $"({_codeBuffer.CursorPositionFromTop}) != {initialCursorPositionFromTop}");
            
            _codeBuffer.IncCursorPositionFromTop();
            
            isCursorPositionFromTopCorrect = _codeBuffer.CursorPositionFromTop == increasedCursorPositionFromTop;
            
            Assert.True(isCursorPositionFromTopCorrect,
                "Cursor position from top isn't correct! " +
                $"({_codeBuffer.CursorPositionFromTop}) != {increasedCursorPositionFromTop}");
        }

        [Test]
        public void CursorPositionFromLeftTest()
        {
            var initialCursorPositionFromLeft = _codeBufferContent.CursorPositionFromLeft;
            var increasedCursorPositionFromLeft = initialCursorPositionFromLeft + 1;
            
            var isCursorPositionFromLeftCorrect = _codeBuffer.CursorPositionFromLeft == initialCursorPositionFromLeft;

            Assert.True(isCursorPositionFromLeftCorrect,
                "Cursor position from left isn't correct! " +
                $"({_codeBuffer.CursorPositionFromLeft}) != {initialCursorPositionFromLeft}");
            
            _codeBuffer.IncCursorPositionFromLeft();
            
            isCursorPositionFromLeftCorrect = _codeBuffer.CursorPositionFromLeft == increasedCursorPositionFromLeft;
            
            Assert.True(isCursorPositionFromLeftCorrect,
                "Cursor position from left isn't correct! " +
                $"({_codeBuffer.CursorPositionFromLeft}) != {increasedCursorPositionFromLeft}");
        }

        [Test]
        public void CurrentLineTest()
        {
            var initialPositionFromTop = 0;
            var supposedLine = _codeBufferContent.SourceCode[initialPositionFromTop];
            var line = _codeBuffer.CurrentLine;

            var isTheLineCorrect = line == supposedLine;

            Assert.True(isTheLineCorrect,
                $"The current line isn't correct! ({line} != {supposedLine})");
            
            _codeBuffer.IncCursorPositionFromTop();

            supposedLine = _codeBufferContent.SourceCode[initialPositionFromTop + 1];
            line = _codeBuffer.CurrentLine;

            isTheLineCorrect = line == supposedLine;
            
            Assert.True(isTheLineCorrect,
                $"The current line isn't correct! ({line} != {supposedLine})");
        }

        [Test]
        public void CodeWithLineNumbersTest()
        {
            var code = _codeBuffer.CodeWithLineNumbers;
            var supposedCode = string.Join("\n",
                _codeBufferContent.SourceCode.Select((l, i) => $"  {i + 1} |{l}"));

            var isTheCodeCorrect = code == supposedCode;
            
            Assert.True(isTheCodeCorrect,
                $"The gotten code with line numbers isn't correct! \n{code}!=\n{supposedCode}");
        }

        [Test]
        public void CodeTest()
        {
            var code = _codeBuffer.Code;
            var supposedCode = string.Join("\n", _codeBufferContent.SourceCode);
            
            var isTheCodeCorrect = code == supposedCode;
            
            Assert.True(isTheCodeCorrect,
                $"The gotten code isn't correct! \n{code}!=\n{supposedCode}");
        }

        [Test]
        public void LinesTest()
        {
            var lines = string.Join("", _codeBuffer.Lines);
            var supposedLines = string.Join("", _codeBufferContent.SourceCode.ToArray());

            var areTheLinesCorrect = lines == supposedLines;

            Assert.True(areTheLinesCorrect,
                $"The gotten lines aren't correct! \n{lines}\n!=\n{supposedLines}");
        }

        [Test]
        public void GetLineAtTest()
        {
            for (var i = 0; i < _codeBufferContent.SourceCode.Count; i++)
            {
                var lineNumber = i + 1;
                var line = _codeBufferContent.SourceCode[i];
                var supposedLine = _codeBuffer.GetLineAt(lineNumber);

                var isTheLineCorrect = line == supposedLine;

                Assert.True(isTheLineCorrect,
                    $"The gotten line (lineNumber={lineNumber}) isn't correct! ({line} != {supposedLine})");
            }
        }
    }

    internal class MockSourceCodeBufferContent : ICodeBufferContent
    {
        public List<string> SourceCode => new(new []
        {
            "--------------------",
            "----------",
            "\\",
            ""
        });

        public int CursorPositionFromTop => 0;

        public int CursorPositionFromLeft => 5;
    }
}
