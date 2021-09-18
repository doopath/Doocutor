using System;
using System.Linq;
using Domain.Core.CodeBuffers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Spectre.Console;

namespace DynamicEditor.Core
{
    internal sealed class CuiRender
    {
        private readonly ICodeBuffer _codeBuffer;
        public int TopOffset { get; set; }
        public int LeftOffset { get; set; }

        public CuiRender(ICodeBuffer codeBuffer)
        {
            _codeBuffer = codeBuffer;
            TopOffset = 0;
        }

        public void Render()
        {
            var code = _codeBuffer.CodeWithLineNumbers;
            var codeLines = code.Split("\n")[TopOffset..];
            var width = Console.WindowWidth;
            var height = Console.WindowHeight;

            DisableCursor();
            Console.SetCursorPosition(0, 0);

            for (var t = 0; t < height - 1; t++)
            {
                var line = codeLines.Length < t + 1 ? new string(' ', width) : codeLines[t];
                line += line.Length < width ? new string(' ', width - line.Length) : "";

                Console.Write(line);
            }

            FixCursorPosition(width, height);
            UpdateCursorPosition();
            EnableCursor();
        }

        public void Clear()
            => Console.Clear();

        public void MoveCursorUp()
        {
            if (_codeBuffer.CursorPositionFromTop - TopOffset == 0 && TopOffset != 0)
                TopOffset--;

            _codeBuffer.DecCursorPositionFromTop();
            Render();
        }

        public void MoveCursorDown()
        {
            _codeBuffer.IncCursorPositionFromTop();
            Render();
        }

        public void MoveCursorLeft()
        {
            // if (_codeBuffer.CursorPositionFromLeft - LeftOffset == _codeBuffer.CurrentLinePrefix.Length - 1 && LeftOffset == 0)
            //     TopOffset--;

            _codeBuffer.DecCursorPositionFromLeft();
            Render();
        }

        public void MoveCursorRight()
        {
            _codeBuffer.IncCursorPositionFromLeft();
            Render();
        }

        private void EnableCursor()
            => Console.CursorVisible = true;

        private void DisableCursor()
            => Console.CursorVisible = false;

        private void FixCursorPosition(int width, int height)
        {
            if (_codeBuffer.CursorPositionFromTop - TopOffset >= height)
            {
                TopOffset++;
            }
            else if (_codeBuffer.CursorPositionFromTop - TopOffset < 0)
            {
                TopOffset--;
            }

            ShowDevMonitor();
        }

        private void UpdateCursorPosition()
            => Console.SetCursorPosition(
            _codeBuffer.CursorPositionFromLeft - LeftOffset,
            _codeBuffer.CursorPositionFromTop - TopOffset);

        private void ShowDevMonitor()
        {
            DisableCursor();
            Console.CursorTop = 1;
            Console.ForegroundColor = ConsoleColor.Cyan;

            var monitor = $"Top: [ offset: {TopOffset}; pos: {_codeBuffer.CursorPositionFromTop} ]\n";
            monitor += $"Left: [ offset: {LeftOffset}; pos: {_codeBuffer.CursorPositionFromLeft} ]";

            var output = monitor.Split("\n").ToList();

            foreach (var l in output)
            {
                Console.CursorLeft = Console.WindowWidth - l.Length - 2;

                Console.Write(l);
                Console.SetCursorPosition(Console.WindowWidth - 1, Console.CursorTop + 1);
            }

            Console.ResetColor();
            UpdateCursorPosition();
            EnableCursor();
        }
    }
}
