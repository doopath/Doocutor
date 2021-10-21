using System;
using System.Collections.Generic;
using Spectre.Console;

namespace Domain.Core.OutBuffers
{
    public class StandardConsoleOutBuffer : IOutBuffer
    {
        public int Width { get => Console.WindowWidth; set => Console.WindowWidth = value; }
        public int Height { get => Console.WindowHeight; set => Console.WindowHeight = value; }
        public bool CursorVisible { get => Console.CursorVisible; set => Console.CursorVisible = value; }

        public void Clear()
            => Console.Clear();

        public void SetCursorPosition(int left, int top)
            => Console.SetCursorPosition(left, top);

        public void Write(string line)
            => Console.Write(line);

        public void Fill(IEnumerable<string> scene)
        {
            foreach (var line in scene)
                Write(line);
        }

        public void WriteLine(string line)
            => Console.WriteLine(line);
    }
}