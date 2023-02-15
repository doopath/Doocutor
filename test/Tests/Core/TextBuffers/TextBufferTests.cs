using Common;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TextBuffer.TextBufferContents;
using Utils.Exceptions.NotExitExceptions;

namespace Tests.Core.TextBuffers
{
    [TestFixture]
    internal class TextBufferTests
    {
        private ITextBuffer? _codeBuffer;
        private ITextBufferContent? _codeBufferContent;

        [SetUp]
        public void Setup()
        {
            _codeBufferContent = new MockSourceCodeBufferContent();
            _codeBuffer = new TextBuffer.TextBuffers.TextBuffer(_codeBufferContent);
            Checkbox.TurnOff();
        }

        [Test]
        public void SizeTest()
        {
            void test(int bufferSize)
            {
                var isSizeOfTheBufferCorrect = _codeBuffer!.Size == bufferSize;

                Assert.True(isSizeOfTheBufferCorrect,
                    $"Size of the code buffer isn't correct! ({_codeBuffer} != {bufferSize})");
            }

            var initialBufferSize = _codeBufferContent!.SourceCode.Count;

            test(initialBufferSize);

            _codeBuffer!.Write(string.Empty);

            test(initialBufferSize + 1);
        }

        [Test]
        public void CursorPositionFromTopTest()
        {
            void test(int cursorPositionFromTop)
            {
                var isCursorPositionFromTopCorrect = _codeBuffer!.CursorPositionFromTop == cursorPositionFromTop;

                Assert.True(isCursorPositionFromTopCorrect,
                    "Cursor position from top isn't correct! " +
                    $"({_codeBuffer.CursorPositionFromTop}) != {cursorPositionFromTop}");
            }

            test(0);

            _codeBuffer!.IncCursorPositionFromTop();

            test(1);
        }

        [Test]
        public void CursorPositionFromLeftTest()
        {
            void test(int cursorPositionFromLeft)
            {
                var isCursorPositionFromLeftCorrect = _codeBuffer!.CursorPositionFromLeft == cursorPositionFromLeft;

                Assert.True(isCursorPositionFromLeftCorrect,
                    "Cursor position from left isn't correct! " +
                    $"({_codeBuffer.CursorPositionFromLeft}) != {cursorPositionFromLeft}");
            }

            test(5);

            _codeBuffer!.IncCursorPositionFromLeft();

            test(6);
        }

        [Test]
        public void CurrentLineTest()
        {
            void test(int cursorPositionFromTop)
            {
                var supposedLine = _codeBufferContent!.SourceCode[cursorPositionFromTop];
                var line = _codeBuffer!.CurrentLine;

                var isTheLineCorrect = line == supposedLine;

                Assert.True(isTheLineCorrect,
                    $"The current line isn't correct! ({line} != {supposedLine})");
            }

            test(0);

            _codeBuffer!.IncCursorPositionFromTop();

            test(1);
        }

        [Test]
        public void CodeWithLineNumbersTest()
        {
            var code = _codeBuffer!.CodeWithLineNumbers;
            var supposedCode = string.Join("\n",
                _codeBufferContent!.SourceCode.Select((l, i) => $"  {i + 1} |{l}"));

            var isTheCodeCorrect = code == supposedCode;

            Assert.True(isTheCodeCorrect,
                $"The gotten code with line numbers isn't correct! \n{code}!=\n{supposedCode}");
        }

        [Test]
        public void CodeTest()
        {
            var code = _codeBuffer!.Text;
            var supposedCode = string.Join("\n", _codeBufferContent!.SourceCode);

            var isTheCodeCorrect = code == supposedCode;

            Assert.True(isTheCodeCorrect,
                $"The gotten code isn't correct! \n{code}!=\n{supposedCode}");
        }

        [Test]
        public void LinesTest()
        {
            var lines = string.Join("", _codeBuffer!.Lines);
            var supposedLines = string.Join("", _codeBufferContent!.SourceCode.ToArray());

            var areTheLinesCorrect = lines == supposedLines;

            Assert.True(areTheLinesCorrect,
                $"The gotten lines aren't correct! \n{lines}\n!=\n{supposedLines}");
        }

        [Test]
        public void GetLineAtTest()
        {
            for (var i = 0; i < _codeBufferContent!.SourceCode.Count; i++)
            {
                var lineNumber = i + 1;
                var line = _codeBufferContent.SourceCode[i];
                var supposedLine = _codeBuffer!.GetLineAt(lineNumber);

                var isTheLineCorrect = line == supposedLine;

                Assert.True(isTheLineCorrect,
                    $"The gotten line (lineNumber={lineNumber}) isn't correct! ({line} != {supposedLine})");
            }
        }

        [Test]
        public void GetCodeBlockTest()
        {
            void test(ITextBlockPointer pointer)
            {
                var supposedCodeBlock = string.Join("", _codeBufferContent!
                    .SourceCode
                    .Skip(pointer.StartLineNumber - 1)
                    .Take(pointer.EndLineNumber - pointer.StartLineNumber));
                var codeBlock = string.Join("", _codeBuffer!.GetTextBlock(pointer));

                var isCodeBlockCorrect = codeBlock == supposedCodeBlock;

                Assert.True(isCodeBlockCorrect,
                    $"The gotten code block isn't correct! \n{codeBlock}\n!=\n{supposedCodeBlock}");
            }

            test(new TextBlockPointer(1, 1));
            test(new TextBlockPointer(1, 4));
            test(new TextBlockPointer(1, 5));
        }

        [Test]
        public void RemoveCodeBlockTest()
        {
            void test(ITextBlockPointer pointer)
            {
                var start = pointer.StartLineNumber;
                var end = pointer.EndLineNumber;
                var codeContent = _codeBufferContent!.SourceCode.ToArray();
                var supposedCode = string.Join("\n",
                    codeContent[..(pointer.StartLineNumber - 1)].Concat(codeContent[(pointer.EndLineNumber - 1)..]));

                _codeBuffer!.RemoveTextBlock(pointer);

                var isTheCodeCorrect = _codeBuffer.Text == supposedCode;

                Assert.True(isTheCodeCorrect,
                    $"The gotten code after removing line since {start} to {end} isn't correct! \n{_codeBuffer.Text}\n!=\n{supposedCode}");

                Setup();
            }

            test(new TextBlockPointer(1, 2));
            test(new TextBlockPointer(1, 3));
            test(new TextBlockPointer(1, 5));
        }

        [Test]
        public void RemoveLineAtTest()
        {
            void test(int lineNumber)
            {
                var supposedCodeLines = _codeBufferContent!.SourceCode;
                var targetIndex = lineNumber - 1;

                // This line should be higher than the one below, because of in the incorrectArgumentTest
                // the RemoveLineAt method must throw an error, but if you place a call of this one lower,
                // then the RemoveAt method will throw the ArgumentOutOfRangeException.
                _codeBuffer!.RemoveLineAt(lineNumber);
                supposedCodeLines.RemoveAt(targetIndex);

                var code = _codeBuffer.Text;
                var supposedCode = string.Join("\n", supposedCodeLines);

                var isTheCodeCorrect = code == supposedCode;

                Assert.True(isTheCodeCorrect,
                    $"The gotten code isn't correct! \n{code}\n!=\n{supposedCode}");

                Setup();
            }

            test(4);
            test(1);
            test(2);
        }

        [Test]
        public void RemoveLineAtIncorrectLineNumberTest()
        {
            void test(int lineNumber)
            {
                try
                {
                    _codeBuffer!.RemoveLineAt(lineNumber);
                }
                catch (OutOfTextBufferSizeException)
                {
                    Checkbox.TurnOn();
                }
                finally
                {
                    Assert.True(Checkbox.State,
                        "The method RemoveLineAt should throw the " +
                        $"OutOfCodeBufferSizeException if an incorrect argument passed ({lineNumber})!");

                    Setup();
                }
            }

            test(5);
            test(0);
        }

        [Test]
        public void IncreaseBufferSizeTest()
        {
            var initialBufferSize = _codeBufferContent!.SourceCode.Count;
            var supposedIncreasedBufferSize = initialBufferSize + 1;
            var initialContent = string.Join("", _codeBufferContent.SourceCode);

            _codeBuffer!.IncreaseBufferSize();

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

                while (_codeBuffer!.Size < linesCount)
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
                var supposedLines = _codeBufferContent!.SourceCode;

                supposedLines[index] = newLine;
                _codeBuffer!.ReplaceLineAt(lineNumber, newLine);

                var supposedCode = string.Join("\n", supposedLines);
                var code = _codeBuffer.Text;

                var isTheCodeCorrect = code == supposedCode;

                Assert.True(isTheCodeCorrect,
                    $"The gotten code isn't correct! \n{code}\n!=\n{supposedCode}");

                Setup();
            }

            test(1, "");
            test(2, "");
            test(1, "new line");
            test(2, "new line");
        }

        [Test]
        public void WriteTest()
        {
            void test(string line)
            {
                var supposedLines = _codeBufferContent!.SourceCode;

                supposedLines.Insert(_codeBufferContent.CursorPositionFromTop, line);
                _codeBuffer!.Write(line);

                var supposedCode = string.Join("\n", supposedLines);
                var code = _codeBuffer.Text;

                var isTheCodeCorrect = code == supposedCode;

                Assert.True(isTheCodeCorrect,
                    $"The gotten code isn't correct! \n{code}\n!=\n{supposedCode}");

                Setup();
            }

            test("");
            test("\\");
            test("new line");
        }

        [Test]
        public void WriteAfterTest()
        {
            void test(int lineNumber, string line)
            {
                var supposedLines = _codeBufferContent!.SourceCode;

                supposedLines.Insert(lineNumber, line);
                _codeBuffer!.WriteAfter(lineNumber, line);

                var supposedCode = string.Join("\n", supposedLines);
                var code = _codeBuffer.Text;

                var isTheCodeCorrect = code == supposedCode;

                Assert.True(isTheCodeCorrect,
                    $"The gotten code isn't correct! \n{code}\n!=\n{supposedCode}");

                Setup();
            }

            test(1, "");
            test(1, "new line");
            test(4, "");
            test(4, "new line");
        }

        [Test]
        public void WriteBeforeTest()
        {
            void test(int lineNumber, string line)
            {
                var supposedLines = _codeBufferContent!.SourceCode;

                supposedLines.Insert(lineNumber - 1, line);
                _codeBuffer!.WriteBefore(lineNumber, line);

                var supposedCode = string.Join("\n", supposedLines);
                var code = _codeBuffer.Text;

                var isTheCodeCorrect = code == supposedCode;

                Assert.True(isTheCodeCorrect,
                    $"The gotten code isn't correct! \n{code}\n!=\n{supposedCode}");

                Setup();

            }

            test(1, "");
            test(1, "new line");
            test(4, "");
            test(4, "new line");
        }

        [Test]
        public void AppendLineTest()
        {
            void test(int top, int left, string newPart, string supposedLine)
            {
                var lineNumber = top + 1;

                _codeBuffer!.SetCursorPositionFromTopAt(top);
                _codeBuffer.SetCursorPositionFromLeftAt(left);
                _codeBuffer.AppendLine(newPart);

                var line = _codeBuffer.GetLineAt(lineNumber);

                var isTheLineCorrect = line == supposedLine;

                Assert.True(isTheLineCorrect,
                    $"The gotten line isn't correct! ({line} != {supposedLine})");

                Setup();
            }

            var prefixLength = _codeBufferContent!.CursorPositionFromLeft;
            var endOfTheFirstLine = _codeBufferContent.SourceCode[0].Length + prefixLength;
            var middleOfTheFirstLine = _codeBufferContent.SourceCode[0].Length / 2 + prefixLength;
            var symbols =
                ("abcdefghijklmnopqrstuvwxyz1234567890-=" +
                 "ABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*()_+" +
                 "[]{}\\|;':\",./<>?").ToArray();

            test(0, prefixLength, "%", "%--------------------");
            test(0, prefixLength, "\\", "\\--------------------");
            test(0, endOfTheFirstLine, "%", "--------------------%");
            test(0, endOfTheFirstLine, "\\", "--------------------\\");
            test(0, middleOfTheFirstLine, "%", "----------%----------");
            test(0, middleOfTheFirstLine, "\\", "----------\\----------");
            test(3, prefixLength, "%", "%");

            foreach (var symbol in symbols)
            {
                var newPart = symbol.ToString();
                test(0, middleOfTheFirstLine, newPart, $"----------{newPart}----------");
            }
        }
    }

    internal sealed record MockSourceCodeBufferContent : ITextBufferContent
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
