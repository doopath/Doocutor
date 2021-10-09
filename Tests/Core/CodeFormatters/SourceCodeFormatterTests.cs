using System;
using System.Collections.Generic;
using Domain.Core.CodeFormatters;
using NUnit.Framework;

namespace Tests.Core.CodeFormatters
{
    [TestFixture]
    public class SourceCodeFormatterTests
    {
        private List<string> _code;
        private ICodeFormatter _formatter;


        [SetUp]
        public void Setup()
        {
            _code = new List<string>();

            FillCode();

            _formatter = new SourceCodeFormatter(_code);
        }

        [TearDown]
        public void Teardown()
        {
            _code = null;
            _formatter = null;
        }

        [Test]
        public void AdaptCodeForBufferSizeTest()
        {
            var maxLineLength = 50;
            var prefixLength = 5;
            var firstLineLength = 72;

            _formatter.AdaptCodeForBufferSize(maxLineLength);

            var hasTargetLinesCount = _code.Count == 10;
            var firstLineIsCorrect = _code[0].Length == maxLineLength - prefixLength;
            var secondLineIsCorrect = _code[1].Length == firstLineLength - (maxLineLength - prefixLength);

            Console.WriteLine(string.Join("\n", _code));

            Assert.True(hasTargetLinesCount, "Adapted code has incorrect line number!");
            Assert.True(firstLineIsCorrect, "First line of code is incorrect!");
            Assert.True(secondLineIsCorrect, "Second line of code is incorrect");
        }

        private void FillCode()
        {
            _code.Add("------------------------------------------------------------------------");
            _code.Add("---------------------------------------------------------");
            _code.Add("-------------------------------------------");
            _code.Add("------------------------------");
            _code.Add("-");
            _code.Add(" ");
            _code.Add("\\");
            _code.Add("");
        }
    }
}