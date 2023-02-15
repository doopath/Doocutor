using NUnit.Framework;
using System.Collections.Generic;
using TextBuffer.Cursors;
using TextBuffer.TextBufferFormatters;
using Utils.Exceptions.NotExitExceptions;

namespace Tests.Core.Cursors;

[TestFixture]
public class TextBufferCursorTests
{
    private ICursor? _cursor;
    private List<string>? _text;
    private ITextBufferFormatter? _formatter;
    private int _initialPositionFromTop;
    private int _initialPositionFromLeft;

    [SetUp]
    public void Setup()
    {
        _text = new List<string>();
        _formatter = new TextBufferFormatter(_text);
        _initialPositionFromTop = 0;
        _initialPositionFromLeft = 0;
        _cursor = new TextBufferCursor(_text, _formatter, _initialPositionFromTop, _initialPositionFromLeft);

        FillCode();
    }

    [Test]
    public void SetCursorPositionFromLeftAtTheStartOfTheFirstLineTest()
    {
        var startOfTheFirstLine = _formatter!.GetPrefixLength(1);

        _cursor!.SetCursorPositionFromLeftAt(startOfTheFirstLine);

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
        var lineLength = _formatter!.GroupOutputLineAt(1)[..^1].Length;

        _cursor!.SetCursorPositionFromLeftAt(lineLength);

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
        var lineLength = _formatter!.GroupOutputLineAt(1)[..^1].Length;

        _cursor!.SetCursorPositionFromLeftAt(lineLength + 1);

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
            _cursor!.SetCursorPositionFromTopAt(-1);
        }
        catch (OutOfTextBufferSizeException)
        {
            var isTheCursorPositionFromTopCorrect = _cursor!.CursorPositionFromTop == 0;
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
            _cursor!.SetCursorPositionFromTopAt(_text!.Count);
        }
        catch (OutOfTextBufferSizeException)
        {
            var isTheCursorPositionFromTopCorrect = _cursor!.CursorPositionFromTop == 0;
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
        var positionOfTheLastSymbolInTheSecondLine = _formatter!
            .GroupOutputLineAt(lineNumberOfTheSecondLine)[..^1]
            .Length;

        _cursor!.SetCursorPositionFromTopAt(positionOfTheSecondLine);
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
        var middlePositionOfTheFirstLine = _formatter!.GroupOutputLineAt(1)[..^1].Length / 2;
        var nextPositionFromTop = 1;

        _cursor!.SetCursorPositionFromLeftAt(middlePositionOfTheFirstLine);
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
        _text!.Add("----------");
        _text.Add("----------");
        _text.Add("\\");
        _text.Add("");
    }
}