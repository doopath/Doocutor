using System;
using System.Collections.Generic;
using Domain.Core;

namespace DynamicEditor.Core
{
    internal static class MovementKeysMap
    {
        public static Dictionary<string, Action> Map = new(
            new[]
            {
                new KeyValuePair<string, Action>("UpArrow", () => CuiRender.MoveCursorUp()),
                new KeyValuePair<string, Action>("DownArrow", () => CuiRender.MoveCursorDown()),
                new KeyValuePair<string, Action>("LeftArrow", () => CuiRender.MoveCursorLeft()),
                new KeyValuePair<string, Action>("RightArrow", () => CuiRender.MoveCursorRight())
            });
    }
}