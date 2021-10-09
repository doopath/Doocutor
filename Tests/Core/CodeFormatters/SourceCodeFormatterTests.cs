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
            var firstFourLinesLength = 45;
            var fifthLineLength = 36;

            _formatter.AdaptCodeForBufferSize(maxLineLength);

            var hasTargetLinesCount = _code.Count == 8;
            var firstLineIsCorrect = _code[0].Length == firstFourLinesLength;
            var secondLineIsCorrect = _code[1].Length == firstFourLinesLength;
            var thirdLineIsCorrect = _code[2].Length == firstFourLinesLength;
            var fourthLineIsCorrect = _code[3].Length == firstFourLinesLength;
            var fifthLineIsCorrect = _code[4].Length == fifthLineLength;

            Console.WriteLine(string.Join("\n", _code));

            Assert.True(hasTargetLinesCount, "Adapted code has incorrect line number!");
            Assert.True(firstLineIsCorrect, "First line of code is incorrect!");
            Assert.True(secondLineIsCorrect, "Second line of code is incorrect");
            Assert.True(thirdLineIsCorrect, "Third line of code is incorrect");
            Assert.True(fourthLineIsCorrect, "Fourth line of code is incorrect");
            Assert.True(fifthLineIsCorrect, "Fifth line of code is incorrect");
        }

        private void FillCode()
        {
            _code.Add("--------------------------------------------------------");
            _code.Add("------------------------------------------------------");
            _code.Add("------------------------------------------------------------");
            _code.Add("---------------------------------------------");
            _code.Add("-");
            _code.Add(" ");
            _code.Add("\\");
            _code.Add("");
        }
    }
}