using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.CodeBufferHistories;
using NUnit.Framework;

namespace Tests.Core.CodeBufferHistories
{
    [TestFixture]
    internal class SourceCodeBufferHistoryTests
    {
        private uint _maxLength;
        private List<string>? _buffer;
        private ICodeBufferHistory? _history;

        [SetUp]
        public void SetUp()
        {
            _maxLength = 100;
            _buffer = new();
            _history = new SourceCodeBufferHistory(_maxLength);
            
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
                
                _history!.Add(new()
                {
                    Range = new(i, i+1), 
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
                CodeBufferChange change = new()
                {
                    Range = range,
                    NewChanges = (string[]) lines.Clone(),
                    OldState = oldState
                };

                _buffer = (bufferBackupArray[..range.Start]
                    .Concat(lines)
                    .Concat(bufferBackupArray[range.End..])).ToList();
                _history!.Add(change);


                change = _history.Undo();
                string[] bufferArray = _buffer.ToArray();
                _buffer = (bufferArray[..change.Range.Start]
                    .Concat(change.OldState)
                    .Concat(bufferArray[(change.Range.Start.Value +
                         change.NewChanges.Length)..])).ToList();

                
                string supposedCode = string.Join("\n", bufferBackup);
                string code = string.Join("\n", _buffer);
                bool isTheBufferCorrect = code == supposedCode;

                Assert.True(isTheBufferCorrect,
                    $"The {nameof(SourceCodeBufferHistory)} cannot handle " +
                    "multiple lines changes correctly. (Method Add)" +
                    $"\n'{code}'\n!=\n'{supposedCode}'\n");

                SetUp();
            }
            
            Test(new(0, 2), new[] { "First Line", "Second Line" });
            Test(new(0, 3), new[] { "", "", "" });
            Test(new(0, 4), new[] { "\\", "\n", "\r", "" });
        }

        private void FillBuffer()
        {
            _buffer?.Add("---------");
            _buffer?.Add("---------");
            _buffer?.Add("\\");
            _buffer?.Add("");
        }
    }
}