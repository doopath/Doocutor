using System.Collections.Generic;

namespace DynamicEditor.Core
{
    public static class KeyCombinationsMap
    {
        public static Dictionary<string, string> Map { get; } = new(
            new[] {
                new KeyValuePair<string, string>("Alt+C", ":showPos"),
                new KeyValuePair<string, string>("Control+Q", ":quit")
            });
    }
}
