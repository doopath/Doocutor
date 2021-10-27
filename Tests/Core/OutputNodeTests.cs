using Domain.Core.CodeBuffers;
using Domain.Core.OutBuffers;
using Domain.Core.Scenes;
using DynamicEditor.Core;
using DynamicEditor.Core.Scenes;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.CodeBufferContents;
using Domain.Core.ColorSchemes;
using Pastel;

namespace Tests.Core
{
    [TestFixture]
    internal class OutputNodeTests
    {
        private const int UpdateRate = 300;
        private IColorScheme _colorScheme;
        private ICodeBuffer _codeBuffer;
        private IOutBuffer _outBuffer;
        private CuiRender _render;
        private IScene _scene;

        [SetUp]
        public void Setup()
        {
            _colorScheme = new DefaultDarkColorScheme();
            _outBuffer = new MockConsole();
            _codeBuffer = new SourceCodeBuffer(new MockCodeBufferContent());
            _scene = new CuiScene();
            _render = new CuiRender(_codeBuffer, _outBuffer, _scene, _colorScheme);
            _render.DisableDeveloperMonitor();
            Checkbox.TurnOff();
            MockConsoleBuffer.ResetBuffer();
        }

        [Test]
        public void RenderTest()
        {
            int top = _codeBuffer.CursorPositionFromTop;
            int left = _codeBuffer.CursorPositionFromLeft;
            string supposedCode = _codeBuffer.CodeWithLineNumbers.Trim();
            List<string> supposedLines = supposedCode.Split("\n").ToList();
            string line = supposedLines[top];

            supposedLines[top] = line[..left] + supposedLines[left]
                .Pastel(_colorScheme.CursorForeground)
                .PastelBg(_colorScheme.CursorBackground) + line[(left + 1)..];
            supposedCode = string.Join("\n", supposedLines);

            _render.Render();

            List<string> lines = MockConsoleBuffer.Content;
            string code = string.Join("\n", lines).Trim();

            bool isRenderedCodeCorrect = code == supposedCode;

            Assert.True(isRenderedCodeCorrect,
                $"The rendered code isn't correct! \n{code}\n!=\n{supposedCode}");
        }
    }

    internal sealed record MockConsole : IOutBuffer
    {
        public int Width { get => MockConsoleBuffer.Width; set => MockConsoleBuffer.Width = value; }
        public int Height { get => MockConsoleBuffer.Height; set => MockConsoleBuffer.Height = value; }
        public bool CursorVisible { get => MockConsoleBuffer.CursorVisible; set => MockConsoleBuffer.CursorVisible = value; }
        public int CursorTop { get => MockConsoleBuffer.CursorTop; set => MockConsoleBuffer.CursorTop = value; }
        public int CursorLeft { get => MockConsoleBuffer.CursorLeft; set => MockConsoleBuffer.CursorLeft = value; }

        public void Clear()
            => MockConsoleBuffer.Content.Clear();

        public void Fill(IEnumerable<string> scene)
        {
            foreach (var line in scene)
                MockConsoleBuffer.Content.Add(line);
        }

        public void SetCursorPosition(int left, int top)
            => (MockConsoleBuffer.CursorTop, MockConsoleBuffer.CursorLeft) = (top, left);

        public void Write(string line)
        {
            var currentLine = MockConsoleBuffer.Content[MockConsoleBuffer.CursorTop];
            var left = MockConsoleBuffer.CursorLeft;

            MockConsoleBuffer.Content[MockConsoleBuffer.CursorTop] =
                currentLine[..left] + line + currentLine[left..];
            MockConsoleBuffer.CursorLeft += line.Length;
        }

        public void WriteLine(string line)
        {
            MockConsoleBuffer.Content.Insert(MockConsoleBuffer.CursorTop, line);
            MockConsoleBuffer.CursorTop++;
            MockConsoleBuffer.CursorLeft = 0;
        }
    }

    internal static class MockConsoleBuffer
    {
        public static int Width { get; set; } = 100;
        public static int Height { get; set; } = 50;
        public static bool CursorVisible { get; set; } = true;
        public static int CursorTop { get; set; } = 0;
        public static int CursorLeft { get; set; } = 0;
        public static List<string> Content { get; private set; } = new();

        public static void ResetBuffer()
        {
            Width = 100;
            Height = 50;
            CursorVisible = true;
            CursorTop = 0;
            CursorLeft = 0;
            Content = new MockCodeBufferContent().SourceCode;
        }
    }

    internal sealed record MockCodeBufferContent : ICodeBufferContent
    {
        public List<string> SourceCode => new(new[]
        {
            "First Line",
            "Second Line",
            "\\",
            ""
        });

        public int CursorPositionFromTop => 0;
        public int CursorPositionFromLeft => 0;
    }
}
