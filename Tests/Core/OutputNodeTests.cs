using Domain.Core.CodeBuffers;
using Domain.Core.OutBuffers;
using Domain.Core.Scenes;
using DynamicEditor.Core;
using DynamicEditor.Core.Scenes;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;

namespace Tests.Core
{
    [TestFixture]
    internal class OutputNodeTests
    {
        private const int UpdateRate = 300;
        private OutBufferSizeHandler _outBufferSizeHandler;
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
            _outBufferSizeHandler = new OutBufferSizeHandler(_outBuffer, _render, UpdateRate);
            Checkbox.TurnOff();
            MockConsoleBuffer.ResetBuffer();
        }

        [Test]
        public void AdaptOutBufferSizeTest()
        {
            void test(int rate, int width, int height)
            {
                // This test my be depended by power of your machine.
                // Just keep it in mind and if the test is failed, try to increase
                // the value of the delay variable or the passed 'rate' value.
                var delay = rate * 2;

                _outBufferSizeHandler.UpdateRate = rate;
                _scene.OnSceneUpdated += _scene => Checkbox.TurnOn();

                _outBufferSizeHandler.Start();

                Thread.Sleep(rate);
                _outBuffer.Width = width;
                _outBuffer.Height = height;
                Thread.Sleep(delay);

                _outBufferSizeHandler.Stop();

                var isTheOutBufferRefreshed = Checkbox.State;

                Assert.True(isTheOutBufferRefreshed,
                    $"The out buffer isn't refreshed after the delay! (rate={rate})");

                Setup();
            }

            test(100, 30, 20);
            test(90, 30, 20);
            test(80, 30, 20);
            test(70, 30, 20);
        }
    }

    internal sealed record MockConsole : IOutBuffer
    {
        public int Width { get => MockConsoleBuffer.Width; set => MockConsoleBuffer.Width = value; }
        public int Height { get => MockConsoleBuffer.Height; set => MockConsoleBuffer.Height = value; }
        public bool CursorVisible { get => MockConsoleBuffer.CursorVisible; set => MockConsoleBuffer.CursorVisible = value; }

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
