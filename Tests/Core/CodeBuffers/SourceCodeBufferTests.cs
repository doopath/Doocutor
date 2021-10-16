using Domain.Core.CodeBufferContents;
using Domain.Core.CodeBuffers;
using NUnit.Framework;
using System.Collections.Generic;

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
            var defaultPrefixLength = 5;
            var increasedCursorPositionFromLeft = defaultPrefixLength;
            
            var isCursorPositionFromLeftCorrect = _codeBuffer.CursorPositionFromLeft == initialCursorPositionFromLeft;

            Assert.True(isCursorPositionFromLeftCorrect,
                "Cursor position from left isn't correct! " +
                $"({_codeBuffer.CursorPositionFromLeft}) != {initialCursorPositionFromLeft}");
            
            _codeBuffer.IncCursorPositionFromTop();
            
            isCursorPositionFromLeftCorrect = _codeBuffer.CursorPositionFromLeft == increasedCursorPositionFromLeft;
            
            Assert.True(isCursorPositionFromLeftCorrect,
                "Cursor position from left isn't correct! " +
                $"({_codeBuffer.CursorPositionFromLeft}) != {increasedCursorPositionFromLeft}");
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

        public int CursorPositionFromLeft => 0;
    }
}
