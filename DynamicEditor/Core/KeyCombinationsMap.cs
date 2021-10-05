using System.Collections.Generic;

namespace DynamicEditor.Core
{
    public static class KeyCombinationsMap
    {
        public static Dictionary<string, string> Map { get; } = new(
            new[] {
                new KeyValuePair<string, string>(@"Control\+Q", ":quit"),
                new KeyValuePair<string, string>(@"Enter", ":enter"),
                new KeyValuePair<string, string>(@"Backspace", ":backspace"),
                new KeyValuePair<string, string>(@".", ":appendLine "),
            });
    }
}
