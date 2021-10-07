using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Domain.Core;
using Domain.Core.CodeBuffers;
using Domain.Core.CodeFormatters;
using NUnit;
using NUnit.Framework;

namespace Tests.Core
{
    [TestFixture]
    public class SourceCodeFormatterTests
    {
        private ICodeFormatter _formatter;
        private ICodeBuffer _buffer;

        [SetUp]
        public void Setup()
        {
            _buffer = new SourceCodeBuffer();
            _buffer.WriteAfter(_buffer.BufferSize, "");

            _formatter = new SourceCodeFormatter(_buffer.Lines.ToList());
        }

        [TearDown]
        public void Teardown()
        {
            _formatter = null;
            _buffer = null;
        }

        [Test]
        public void GroupOutputLineAtTest()
        {
            var lineNumber = _buffer.BufferSize;
            var lineIndex = _formatter.LineNumberToIndex(lineNumber);
            var line = _formatter.GroupOutputLineAt(lineNumber);
            var expectedLine = $"  {lineNumber} |{_formatter.SourceCode[lineIndex]}\n";
            var condition = line == expectedLine;

            Assert.True(condition, "The 'GroupOutputLineAt' method must work correctly with an empty line.");
        }
    }
}