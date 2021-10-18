using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.CodeBufferContents;
using Domain.Core.CodeBuffers;
using Domain.Core.CodeBuffers.CodePointers;
using Domain.Core.Exceptions;
using NUnit.Framework;

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
            Checkbox.TurnOff();
        }

        [Test]
        public void SizeTest()
        {
            void test(int bufferSize)
            {
                var isSizeOfTheBufferCorrect = _codeBuffer.Size == bufferSize;

                Assert.True(isSizeOfTheBufferCorrect,
                    $"Size of the code buffer isn't correct! ({_codeBuffer} != {bufferSize})");
            }

            var initialBufferSize = _codeBufferContent.SourceCode.Count;

            test(initialBufferSize);

            _codeBuffer.Write(string.Empty);

            test(initialBufferSize + 1);
        }

        [Test]
        public void CursorPositionFromTopTest()
        {
            void test(int cursorPositionFromTop)
            {
                var isCursorPositionFromTopCorrect = _codeBuffer.CursorPositionFromTop == cursorPositionFromTop;

                Assert.True(isCursorPositionFromTopCorrect,
                    "Cursor position from top isn't correct! " +
                    $"({_codeBuffer.CursorPositionFromTop}) != {cursorPositionFromTop}");
            }

            test(0);

            _codeBuffer.IncCursorPositionFromTop();

            test(1);
        }

        [Test]
        public void CursorPositionFromLeftTest()
        {
            void test(int cursorPositionFromLeft)
            {
                var isCursorPositionFromLeftCorrect = _codeBuffer.CursorPositionFromLeft == cursorPositionFromLeft;

                Assert.True(isCursorPositionFromLeftCorrect,
                    "Cursor position from left isn't correct! " +
                    $"({_codeBuffer.CursorPositionFromLeft}) != {cursorPositionFromLeft}");
            }

            test(5);

            _codeBuffer.IncCursorPositionFromLeft();

            test(6);
        }

        [Test]
        public void CurrentLineTest()
        {
            void test(int cursorPositionFromTop)
            {
                var supposedLine = _codeBufferContent.SourceCode[cursorPositionFromTop];
                var line = _codeBuffer.CurrentLine;

                var isTheLineCorrect = line == supposedLine;

                Assert.True(isTheLineCorrect,
                    $"The current line isn't correct! ({line} != {supposedLine})");
            }

            test(0);

            _codeBuffer.IncCursorPositionFromTop();

            test(1);
        }

        [Test]
        public void CodeWithLineNumbersTest()
        {
            var code = _codeBuffer.CodeWithLineNumbers;
            var supposedCode = string.Join("\n",
                _codeBufferContent.SourceCode.Select((l, i) => $"  {i + 1} |{l}"));

            var isTheCodeCorrect = code == supposedCode;

            Assert.True(isTheCodeCorrect,
                $"The gotten code with line numbers isn't correct! \n{code}!=\n{supposedCode}");
        }

        [Test]
        public void CodeTest()
        {
            var code = _codeBuffer.Code;
            var supposedCode = string.Join("\n", _codeBufferContent.SourceCode);

            var isTheCodeCorrect = code == supposedCode;

            Assert.True(isTheCodeCorrect,
                $"The gotten code isn't correct! \n{code}!=\n{supposedCode}");
        }

        [Test]
        public void LinesTest()
        {
            var lines = string.Join("", _codeBuffer.Lines);
            var supposedLines = string.Join("", _codeBufferContent.SourceCode.ToArray());

            var areTheLinesCorrect = lines == supposedLines;

            Assert.True(areTheLinesCorrect,
                $"The gotten lines aren't correct! \n{lines}\n!=\n{supposedLines}");
        }

        [Test]
        public void GetLineAtTest()
        {
            for (var i = 0; i < _codeBufferContent.SourceCode.Count; i++)
            {
                var lineNumber = i + 1;
                var line = _codeBufferContent.SourceCode[i];
                var supposedLine = _codeBuffer.GetLineAt(lineNumber);

                var isTheLineCorrect = line == supposedLine;

                Assert.True(isTheLineCorrect,
                    $"The gotten line (lineNumber={lineNumber}) isn't correct! ({line} != {supposedLine})");
            }
        }

        [Test]
        public void GetCodeBlockTest()
        {
            void test(ICodeBlockPointer pointer)
            {
                var supposedCodeBlock = string.Join("", _codeBufferContent
                    .SourceCode
                    .Skip(pointer.StartLineNumber - 1)
                    .Take(pointer.EndLineNumber - pointer.StartLineNumber));
                var codeBlock = string.Join("", _codeBuffer.GetCodeBlock(pointer));

                var isCodeBlockCorrect = codeBlock == supposedCodeBlock;

                Assert.True(isCodeBlockCorrect,
                    $"The gotten code block isn't correct! \n{codeBlock}\n!=\n{supposedCodeBlock}");
            }

            test(new CodeBlockPointer(1, 1));
            test(new CodeBlockPointer(1, 4));
            test(new CodeBlockPointer(1, 5));
        }

        [Test]
        public void RemoveCodeBlockTest()
        {
            void test(ICodeBlockPointer pointer)
            {
                var start = pointer.StartLineNumber;
                var end = pointer.EndLineNumber;
                var codeContent = _codeBufferContent.SourceCode.ToArray();
                var supposedCode = string.Join("\n",
                    codeContent[..(pointer.StartLineNumber - 1)].Concat(codeContent[(pointer.EndLineNumber - 1)..]));

                _codeBuffer.RemoveCodeBlock(pointer);

                var isTheCodeCorrect = _codeBuffer.Code == supposedCode;

                Assert.True(isTheCodeCorrect,
                    $"The gotten code after removing line since {start} to {end} isn't correct! \n{_codeBuffer.Code}\n!=\n{supposedCode}");
            }

            test(new CodeBlockPointer(1, 2));
            Setup();
            test(new CodeBlockPointer(1, 3));
            Setup();
            test(new CodeBlockPointer(1, 5));
        }

        [Test]
        public void RemoveLineAtTest()
        {
            void test(int lineNumber)
            {
                var supposedCodeLines = _codeBufferContent.SourceCode;
                var targetIndex = lineNumber - 1;

                // This line should be higher than the one below, because of in the incorrectArgumentTest
                // the RemoveLineAt method must throw an error, but if you place a call of this one lower,
                // then the RemoveAt method will throw the ArgumentOutOfRangeException.
                _codeBuffer.RemoveLineAt(lineNumber);
                supposedCodeLines.RemoveAt(targetIndex);

                var code = _codeBuffer.Code;
                var supposedCode = string.Join("\n", supposedCodeLines);

                var isTheCodeCorrect = code == supposedCode;

                Assert.True(isTheCodeCorrect,
                    $"The gotten code isn't correct! \n{code}\n!=\n{supposedCode}");
            }

            test(4);
            Setup();
            test(1);
            Setup();
            test(2);
            Setup();
        }

        [Test]
        public void RemoveLineAtIncorrectLineNumberTest()
        {
            void test(int lineNumber)
            {
                try
                {
                    _codeBuffer.RemoveLineAt(lineNumber);
                }
                catch (OutOfCodeBufferSizeException)
                {
                    Checkbox.TurnOn();
                }
                finally
                {
                    Assert.True(Checkbox.State,
                        "The method RemoveLineAt should throw the " +
                        $"OutOfCodeBufferSizeException if an incorrect argument passed ({lineNumber})!");
                }
            }

            test(5);
            Setup();
            test(0);
        }

        [Test]
        public void IncreaseBufferSizeTest()
        {
            var initialBufferSize = _codeBufferContent.SourceCode.Count;
            var supposedIncreasedBufferSize = initialBufferSize + 1;
            var initialContent = string.Join("", _codeBufferContent.SourceCode);

            _codeBuffer.IncreaseBufferSize();

            var content = string.Join("", _codeBuffer.Lines);

            var isTheContentCorrect = content == initialContent;
            var isTheBufferSizeCorrect = _codeBuffer.Size == supposedIncreasedBufferSize;

            Assert.True(isTheContentCorrect,
                $"The gotten content isn't correct! '{content}' != '{initialContent}'");
            Assert.True(isTheBufferSizeCorrect,
                $"Size of the buffer isn't correct! ({_codeBuffer.Size} != {supposedIncreasedBufferSize})");
        }

        [Test]
        public void GetPrefixLengthTest()
        {
            void test(int linesCount)
            {
                // The prefix for line at line number < 10 is '  x |', so its
                // length equals 5. If we want to get max length of the prefix,
                // we should replace the 'x' (a number) by the max line number.
                // If the count of lines is 100: '  x |' => '  100 |'.
                var supposedPrefixLength = 5 + linesCount.ToString().Length - 1;

                while (_codeBuffer.Size < linesCount)
                    _codeBuffer.IncreaseBufferSize();

                var prefixLength = _codeBuffer.GetPrefixLength();
                var isThePrefixLengthCorrect = prefixLength == supposedPrefixLength;

                Assert.True(isThePrefixLengthCorrect,
                    $"Length of the prefix isn't correct! Buffer's size is {linesCount}. ({prefixLength} != {supposedPrefixLength})");
            }

            test(0);
            test(10);
            test(100);
            test(1000);
        }

        [Test]
        public void ReplaceLineAtTest()
        {
            void test(int lineNumber, string newLine)
            {
                var index = lineNumber - 1;
                var supposedLines = _codeBufferContent.SourceCode;

                supposedLines[index] = newLine;
                _codeBuffer.ReplaceLineAt(lineNumber, newLine);

                var supposedCode = string.Join("\n", supposedLines);
                var code = _codeBuffer.Code;

                var isTheCodeCorrect = code == supposedCode;

                Assert.True(isTheCodeCorrect,
                    $"The gotten code isn't correct! \n{code}\n!=\n{supposedCode}");
            }

            test(1, "");
            Setup();
            test(2, "");
            Setup();
            test(1, "new line");
            Setup();
            test(2, "new line");
        }

        [Test]
        public void WriteTest()
        {
            void test(string line)
            {
                var supposedLines = _codeBufferContent.SourceCode;

                supposedLines.Insert(_codeBufferContent.CursorPositionFromTop, line);
                _codeBuffer.Write(line);

                var supposedCode = string.Join("\n", supposedLines);
                var code = _codeBuffer.Code;

                var isTheCodeCorrect = code == supposedCode;

                Assert.True(isTheCodeCorrect,
                    $"The gotten code isn't correct! \n{code}\n!=\n{supposedCode}");
            }

            test("");
            Setup();
            test("\\");
            Setup();
            test("new line");
        }
    }

    internal sealed record MockSourceCodeBufferContent : ICodeBufferContent
    {
        public List<string> SourceCode => new(new[]
        {
            "--------------------",
            "----------",
            "\\",
            ""
        });

        public int CursorPositionFromTop => 0;

        public int CursorPositionFromLeft => 5;
    }
}
