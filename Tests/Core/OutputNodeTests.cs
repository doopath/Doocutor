using Domain.Core.CodeBuffers;
using Domain.Core.OutBuffers;
using Domain.Core.Scenes;
using DynamicEditor.Core;
using DynamicEditor.Core.Scenes;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests.Core
{
    [TestFixture]
    internal class OutputNodeTests
    {
        private const int UpdateRate = 300;
        private ICodeBuffer _codeBuffer;
        private IOutBuffer _outBuffer;
        private CuiRender _render;
        private IScene _scene;

        [SetUp]
        public void Setup()
        {
            _outBuffer = new MockConsole();
            _codeBuffer = new SourceCodeBuffer();
            _scene = new CuiScene();
            _render = new CuiRender(_codeBuffer, _outBuffer, _scene);
            Checkbox.TurnOff();
            MockConsoleBuffer.ResetBuffer();
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
            Content = new List<string>();
        }
    }
}
