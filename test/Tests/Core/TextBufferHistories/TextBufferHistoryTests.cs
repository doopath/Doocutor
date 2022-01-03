using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using NUnit.Framework;
using TextBuffer.TextBufferHistories;

namespace Tests.Core.TextBufferHistories;

[TestFixture]
internal class TextBufferHistoryTests
{
    private uint _maxLength;
    private List<string>? _buffer;
    private CursorPosition? _oldCursorPosition;
    private CursorPosition? _newCursorPosition;
    private ITextBufferHistory? _history;

    [SetUp]
    public void SetUp()
    {
        _maxLength = 100;
        _buffer = new();
        _oldCursorPosition = new() { Left = 0, Top = 0 };
        _newCursorPosition = new() { Left = 0, Top = 0 };
        _history = new TextBufferHistory(_maxLength);

        FillBuffer();
    }

    [TearDown]
    public void TearDown()
    {
        _maxLength = 0;
        _buffer = null;
        _history = null;
    }

    [Test]
    public void AddOneLineTest()
    {
        List<string> bufferBackup = new(_buffer!);

        for (int i = 0; i < _buffer!.Count; i++)
        {
            string newLine = "+++++++++";

            _history!.Add(new TextBufferChange()
            {
                Range = new(i, i + 1),
                OldCursorPosition = _oldCursorPosition!.Value,
                NewCursorPosition = _newCursorPosition!.Value,
                NewChanges = new[] { _buffer[i] },
                OldState = new[] { bufferBackup[i] }
            });
            _buffer[i] = newLine;
        }

        foreach (var change in _history!)
        {
            string line = change.OldState[0];
            string supposedLine = bufferBackup[change.Range.Start];

            Assert.True(line == supposedLine,
                $"The change committed to the history isn't correct!");
        }
    }

    [Test]
    public void AddMultipleLinesTest()
    {
        void Test(Range range, string[] lines)
        {
            List<string> bufferBackup = new(_buffer!);
            string[] bufferBackupArray = bufferBackup.ToArray();
            string[] oldState = bufferBackupArray[range];
            ITextBufferChange change = new TextBufferChange()
            {
                Range = range,
                OldCursorPosition = _oldCursorPosition!.Value,
                NewCursorPosition = _newCursorPosition!.Value,
                NewChanges = (string[])lines.Clone(),
                OldState = oldState
            };

            ModifyBuffer(range, lines);
            _history!.Add(change);

            change = _history.Undo();
            range = new(change.Range.Start, change.Range.Start.Value
                    + change.NewChanges.Length);

            ModifyBuffer(range, change.OldState);

            string supposedCode = string.Join("\n", bufferBackup);
            string code = string.Join("\n", _buffer!);
            bool isTheBufferCorrect = code == supposedCode;

            Assert.True(isTheBufferCorrect,
                $"The {nameof(TextBufferHistory)} cannot handle " +
                "multiple lines changes correctly. (Method Add)" +
                $"\n'{code}'\n!=\n'{supposedCode}'\n");

            SetUp();
        }

        Test(new(0, 1), new[] { "First Line" });
        Test(new(0, 2), new[] { "First Line", "Second Line" });
        Test(new(0, 3), new[] { "", "", "" });
        Test(new(0, 4), new[] { "\\", "\n", "\r", "" });
    }

    [Test]
    public void UndoTest()
    {
        string[] bufferBackup = _buffer!.ToArray();

        for (int i = 0; i < _buffer.Count; i++)
        {
            string newLine = $"New line {i}";
            ITextBufferChange change = new TextBufferChange()
            {
                Range = new(i, i + 1),
                OldCursorPosition = _oldCursorPosition!.Value,
                NewCursorPosition = _newCursorPosition!.Value,
                NewChanges = new[] { newLine },
                OldState = new[] { _buffer[i] }
            };

            _buffer[i] = newLine;
            _history!.Add(change);
        }

        for (int i = bufferBackup.Length - 1; i >= 0; i--)
            _buffer[i] = _history!.Undo().OldState[0];

        string supposedCode = string.Join("\n", bufferBackup);
        string code = string.Join("\n", _buffer);
        bool isTheCodeCorrect = code == supposedCode;

        Assert.True(isTheCodeCorrect,
            $"The {nameof(TextBufferHistory)} saved the" +
            $"changes incorrect! \n'{code}'\n!=\n'{supposedCode}'\n");
    }

    [Test]
    public void RedoTest()
    {
        void Test(string[][] newChangesArray)
        {
            string[] oldStatesArray = _buffer!.ToArray();

            if (oldStatesArray.Length != newChangesArray.Length)
                throw new IndexOutOfRangeException(
                    "Lengths of the newChangesArray and oldStatesArray should be equal!");

            for (int i = 0; i < newChangesArray.Length; i++)
            {
                _history!.Add(new TextBufferChange()
                {
                    Range = new(i, i + 1),
                    OldCursorPosition = _oldCursorPosition!.Value,
                    NewCursorPosition = _newCursorPosition!.Value,
                    OldState = new[] { oldStatesArray[i] },
                    NewChanges = newChangesArray[i]
                });
            }

            string supposedCode = string.Join("\n", _buffer);
            List<string> newBuffer = new();

            for (int i = 0; i < newChangesArray.Length; i++)
                _history!.Undo();

            for (int i = newChangesArray.Length - 1; i >= 0; i--)
            {
                newBuffer = newBuffer.Concat(_history!.Redo().OldState).ToList();
            }

            string code = string.Join("\n", newBuffer);
            bool isTheCodeCorrect = code == supposedCode;

            Assert.True(isTheCodeCorrect,
                $"The `Redo` method does redo changes incorrectly!" +
                $"\n'{code}'\n!=\n'{supposedCode}'\n");

            SetUp();
        };

        Test(new[]
        {
                new[] { "First Line" },
                new[] { "First Line", "Second Line" },
                new[] { "First LIne", "Second Line", "Third Line" },
                new[] { string.Empty, string.Empty, string.Empty, string.Empty }
            });
    }

    [Test]
    public void LimitOverflowTest()
    {
        void Test(uint limit, int overflowValue)
        {
            _history!.Limit = limit;

            for (var i = 0; i < limit + overflowValue; i++)
                _history.Add(new TextBufferChange()
                {
                    Range = new(0, 1),
                    OldCursorPosition = _oldCursorPosition!.Value,
                    NewCursorPosition = _newCursorPosition!.Value,
                    OldState = new[] { string.Empty },
                    NewChanges = new[] { i.ToString() }
                });

            string firstChange;

            foreach (var change in _history)
                firstChange = change.NewChanges[0];

            string supposedFirstChange = (overflowValue - 1).ToString();
            bool isHistorySizeCorrect = _history.Size == limit;

            Assert.True(isHistorySizeCorrect,
                "Size of the history isn't equals the set limit!" +
                $" ({_history.Size} !+ {limit})");

            SetUp();
        }

        Test(1, 1);
        Test(1, 2);

        Test(10, 1);
        Test(10, 2);

        Test(100, 1);
        Test(100, 2);
    }

    private void ModifyBuffer(Range range, string[] lines)
    {
        int end = range.End.Value;
        int start = range.Start.Value;

        if (_buffer!.Count < (end > start ? end : start))
            throw new IndexOutOfRangeException(
                "Length of the buffer is less than the greatest value of the range.");

        string[] bufferArray = _buffer!.ToArray();

        _buffer = bufferArray[..range.Start]
            .Concat(lines)
            .Concat(bufferArray[range.End..]).ToList();
    }

    private void FillBuffer()
    {
        _buffer?.Add("----/----");
        _buffer?.Add("---------");
        _buffer?.Add("\\");
        _buffer?.Add("");
    }
}
