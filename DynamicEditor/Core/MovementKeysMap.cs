using System;
using System.Collections.Generic;

namespace DynamicEditor.Core
{
    internal static class MovementKeysMap
    {
        public static Dictionary<string, Action<CuiRender>> Map = new(
            new[]
            {
                new KeyValuePair<string, Action<CuiRender>>("UpArrow", r => r.MoveCursorUp()),                
                new KeyValuePair<string, Action<CuiRender>>("DownArrow", r => r.MoveCursorDown()),
                new KeyValuePair<string, Action<CuiRender>>("LeftArrow", r => r.MoveCursorLeft()),
                new KeyValuePair<string, Action<CuiRender>>("RightArrow", r => r.MoveCursorRight())
            });
    }
}