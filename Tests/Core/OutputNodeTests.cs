﻿using System.Collections.Generic;
using System.Linq;
using Domain.Core.ColorSchemes;
using Domain.Core.Cursors;
using Domain.Core.OutBuffers;
using Domain.Core.Scenes;
using Domain.Core.TextBufferContents;
using Domain.Core.TextBuffers;
using DynamicEditor.Core;
using DynamicEditor.Core.Scenes;
using NUnit.Framework;
using Pastel;

namespace Tests.Core
{
    [TestFixture]
    internal class OutputNodeTests
    {
        private const int UpdateRate = 300;
        private IColorScheme? _colorScheme;
        private ITextBuffer? _codeBuffer;
        private IOutBuffer? _outBuffer;
        private CuiRender? _render;
        private IScene? _scene;

        [SetUp]
        public void Setup()
        {
            _colorScheme = new DefaultDarkColorScheme();
            _outBuffer = new MockConsole();
            _codeBuffer = new TextBuffer(new MockCodeBufferContent());
            _scene = new CuiScene();
            _render = new CuiRender(_codeBuffer, _outBuffer, _scene, _colorScheme);
            _render.DisableDeveloperMonitor();
            Checkbox.TurnOff();
            MockConsoleBuffer.ResetBuffer();
        }

        [Test]
        public void RenderTest()
        {
            int top = _codeBuffer!.CursorPositionFromTop;
            int left = _codeBuffer.CursorPositionFromLeft;
            int width = _outBuffer!.Width;
            string supposedCode = _codeBuffer.CodeWithLineNumbers;
            List<string> supposedLines = supposedCode.Split("\n").ToList();
            string lastLine = supposedLines[^1];

            supposedLines = supposedLines
                .Select(l => l + (l.Length < width ? new string(' ', width - l.Length) : string.Empty))
                .SkipLast(1)
                .ToList();
            supposedLines.Add(lastLine);
            supposedLines[top] = supposedLines[top][..left] + supposedLines[top][left]
                .ToString()
                .Pastel(_colorScheme!.CursorForeground)
                .PastelBg(_colorScheme.CursorBackground) + supposedLines[top][left..];
            supposedCode = string.Join("", supposedLines);

            _render!.Render();

            List<string> lines = MockConsoleBuffer.Content;
            string code = string.Join("\n", lines).Trim();
            bool isRenderedCodeCorrect = code == supposedCode;

            Assert.True(isRenderedCodeCorrect,
                $"The rendered code isn't correct! \n{code}\n!=\n{supposedCode}");
        }
    }

    internal sealed record MockConsole : IOutBuffer
    {
        public int Width
        {
            get => MockConsoleBuffer.Width;
            set => MockConsoleBuffer.Width = value;
        }
        public int Height
        {
            get => MockConsoleBuffer.Height;
            set => MockConsoleBuffer.Height = value;
        }
        public bool CursorVisible
        {
            get => MockConsoleBuffer.CursorVisible;
            set => MockConsoleBuffer.CursorVisible = value;
        }
        public int CursorTop
        {
            get => MockConsoleBuffer.CursorTop;
            set => MockConsoleBuffer.CursorTop = value;
        }
        public int CursorLeft
        {
            get => MockConsoleBuffer.CursorLeft;
            set => MockConsoleBuffer.CursorLeft = value;
        }

        public void Clear()
            => MockConsoleBuffer.Content.Clear();

        public void Fill(IEnumerable<string> scene)
        {
            foreach (var line in scene)
                MockConsoleBuffer.Content.Add(line);
        }

        public void SetCursorPosition(int left, int top)
            => (MockConsoleBuffer.CursorTop, MockConsoleBuffer.CursorLeft) = (top, left);

        public void SetCursorPosition(CursorPosition position)
            => (MockConsoleBuffer.CursorTop, MockConsoleBuffer.CursorLeft) = (position.Top, position.Left);

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
            Content = new();
        }
    }

    internal sealed record MockCodeBufferContent : ITextBufferContent
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
