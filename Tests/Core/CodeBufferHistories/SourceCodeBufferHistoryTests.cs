using System.Collections.Generic;
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
        public void AddTest()
        {
            List<string> bufferBackup = new(_buffer!);

            for (int i = 0; i < _buffer!.Count; i++)
            {
                string newLine = "+++++++++";
                
                _history!.Add(new()
                {
                    LineIndex = i, 
                    Changes = new[] {_buffer[i]}
                });
                _buffer[i] = newLine;
            }

            foreach (var change in _history!)
            {
                string line = change.Changes[0];
                string supposedLine = bufferBackup[change.LineIndex];
                
                Assert.True(line == supposedLine,
                    $"The change commited to the history isn't correct!");
            }
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