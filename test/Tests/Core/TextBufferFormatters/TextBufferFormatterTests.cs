using NUnit.Framework;
using System;
using System.Collections.Generic;
using TextBuffer.TextBufferFormatters;

namespace Tests.Core.TextBufferFormatters;

[TestFixture]
internal class TextBufferFormatterTests
{
    private List<string>? _code;
    private ITextBufferFormatter? _formatter;

    [SetUp]
    public void Setup()
    {
        _code = new List<string>();

        FillCode();

        _formatter = new TextBufferFormatter(_code);
    }

    [Test]
    public void AdaptCodeForBufferSizeTest()
    {
        var maxLineLength = 50;
        var firstFourLinesLength = 45;
        var fifthLineLength = 36;

        _formatter!.AdaptCodeForBufferSize(maxLineLength);

        var doesHaveTargetLinesCount = _code!.Count == 8;
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

    [Test]
    public void GroupNewLineOfACurrentOneTest()
    {
        var firstLine = _code![0];

        var prefixLength = _formatter!.GetPrefixLength(1);
        var cursorPositionFromTop = 0;
        var cursorPositionFromLeft = (firstLine.Length + prefixLength) / 2;

        var additionalSymbol = "|";

        var newLine = _formatter.GroupNewLineOfACurrentOne(
            additionalSymbol, cursorPositionFromTop, cursorPositionFromLeft)[prefixLength..];
        var supposedNewLine = firstLine.Insert(
            cursorPositionFromLeft - prefixLength, additionalSymbol);

        var isTheNewLineCorrect = newLine == supposedNewLine;

        Assert.True(isTheNewLineCorrect, $"The new line isn't correct! ({newLine} != {supposedNewLine})");
    }

    [Test]
    public void GroupOutputLineAtTest()
    {
        var index = 4;
        var lineNumber = 4 + 1;
        var pureLine = _code![index];
        var outputLine = _formatter!.GroupOutputLineAt(lineNumber);
        var supposedLine = $"  {lineNumber} |{pureLine}\n";

        var isTheOutputLineCorrect = outputLine == supposedLine;

        Assert.True(isTheOutputLineCorrect,
            $"The gotten output line isn't correct! ({outputLine} != {supposedLine})");
    }

    [Test]
    public void SeparateLineFromLineNumberTest()
    {
        var content = "line content";
        var line = $"  1 |{content}";

        var separatedLineContent = _formatter!.SeparateLineFromLineNumber(line);

        var isTheSeparatedLineContentCorrect = separatedLineContent == content;

        Assert.True(isTheSeparatedLineContentCorrect,
            $"The separated line content isn't correct! ({separatedLineContent} != {content})");
    }

    [Test]
    public void GetSourceCodeWithLineNumbersTest()
    {
        var supposedCodeWithLineNumbers = string.Join("\n", new List<string>()
            {
                "  1 |--------------------------------------------------------",
                "  2 |------------------------------------------------------",
                "  3 |------------------------------------------------------------",
                "  4 |---------------------------------------------",
                "  5 |-",
                "  6 | ",
                "  7 |\\",
                "  8 |"
            });

        var codeWithLineNumbers = _formatter!.GetSourceCodeWithLineNumbers();

        var isTheCodeWithLineNumbersCorrect = codeWithLineNumbers == supposedCodeWithLineNumbers;

        Assert.True(isTheCodeWithLineNumbersCorrect,
            $"The gotten code with line numbers isn't correct! \n{codeWithLineNumbers}\n!=\n{supposedCodeWithLineNumbers}");
    }

    [Test]
    public void GetLineAtTest()
    {
        var index = 0;
        var lineNumber = index + 1;

        var supposedLine = _code![index];
        var line = _formatter!.GetLineAt(lineNumber);

        var isTheLineCorrect = line == supposedLine;

        Assert.True(isTheLineCorrect, $"The gotten line isn't correct! ({line} != {supposedLine})");
    }

    [Test]
    public void GetSourceCodeTest()
    {
        var supposedCode = string.Join("\n", _code!);
        var code = _formatter!.GetSourceCode();

        var isTheCodeCorrect = code == supposedCode;

        Assert.True(isTheCodeCorrect,
            $"The gotten code isn't correct! (\n{code}\n!=\n{supposedCode})");
    }

    [Test]
    public void IndexToLineNumberTest()
    {
        var index = 0;
        var supposedLineNumber = index + 1;
        var lineNumber = _formatter!.IndexToLineNumber(index);

        var isTheLineNumberCorrect = lineNumber == supposedLineNumber;

        Assert.True(isTheLineNumberCorrect,
            $"The gotten line number isn't correct! ({lineNumber} != {supposedLineNumber})");
    }

    [Test]
    public void LineNumberToIndexTest()
    {
        var lineNumber = 1;
        var supposedIndex = lineNumber - 1;
        var index = _formatter!.LineNumberToIndex(lineNumber);

        var isTheIndexCorrect = index == supposedIndex;

        Assert.True(isTheIndexCorrect, $"The gotten index isn't correct! ({index} != {supposedIndex})");
    }

    [Test]
    public void GetPrefixLengthTest()
    {
        var lineNumber = 1;
        var supposedPrefixLength = "  x |".Length;
        var prefixLength = _formatter!.GetPrefixLength(lineNumber);
        var isThePrefixLengthCorrect = prefixLength == supposedPrefixLength;

        Assert.True(isThePrefixLengthCorrect,
            $"The gotten prefix length isn't correct! ({prefixLength} != {supposedPrefixLength})");

        while (_code!.Count < 10)
            _code.Add("-");

        supposedPrefixLength++;
        prefixLength = _formatter.GetPrefixLength(lineNumber);

        isThePrefixLengthCorrect = prefixLength == supposedPrefixLength;

        Assert.True(isThePrefixLengthCorrect,
            $"The gotten prefix length isn't correct! ({prefixLength} != {supposedPrefixLength})");
    }

    private void FillCode()
    {
        _code!.Add("--------------------------------------------------------");
        _code.Add("------------------------------------------------------");
        _code.Add("------------------------------------------------------------");
        _code.Add("---------------------------------------------");
        _code.Add("-");
        _code.Add(" ");
        _code.Add("\\");
        _code.Add("");
    }
}