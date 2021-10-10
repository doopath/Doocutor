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

            var doesHaveTargetLinesCount = _code.Count == 8;
            var isTheFirstLineCorrect = _code[0].Length == firstFourLinesLength;
            var isTheSecondLineCorrect = _code[1].Length == firstFourLinesLength;
            var isTheThirdLineCorrect = _code[2].Length == firstFourLinesLength;
            var isTheFourthLineCorrect = _code[3].Length == firstFourLinesLength;
            var isTheFifthLineCorrect = _code[4].Length == fifthLineLength;

            Console.WriteLine(string.Join("\n", _code));

            Assert.True(doesHaveTargetLinesCount, "Adapted code has an incorrect line number!");
            Assert.True(isTheFirstLineCorrect, "The first line of code isn't correct!");
            Assert.True(isTheSecondLineCorrect, "The second line of code isn't correct!");
            Assert.True(isTheThirdLineCorrect, "The third line of code isn't correct!");
            Assert.True(isTheFourthLineCorrect, "The fourth line of code isn't correct!");
            Assert.True(isTheFifthLineCorrect, "The fifth line of code isn't correct!");
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