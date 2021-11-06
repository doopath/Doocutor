using System.Collections.Generic;

namespace DynamicEditor.Core
{
    public static class KeyCombinationsMap
    {
        public static Dictionary<string, string> Map { get; } = new(
            new[] {
                new KeyValuePair<string, string>(@"Control\+Q", ":quit"),
                new KeyValuePair<string, string>(@"Enter", ":enter"),
                new KeyValuePair<string, string>(@"(Divide|Add|Subtract|Multiply)", ":appendLine "),
                new KeyValuePair<string, string>(@"Backspace", ":backspace"),
                new KeyValuePair<string, string>(@"F[0-9]+", ":doNothing"),
                new KeyValuePair<string, string>(@"(Page[Down|Up]|Home|End|Insert)", ":doNothing"),
                new KeyValuePair<string, string>(@"^(Shift\+)?.{1}$", ":appendLine "),
                new KeyValuePair<string, string>(@"^(Shift\\+)?Tab$", ":tab"),
                new KeyValuePair<string, string>(@"(Shift\+)?(Spacebar|Oem[a-zA-Z0-9])", ":appendLine "),
                new KeyValuePair<string, string>(@"(Shift\+)?D[0-9]+", ":appendLine "),
                new KeyValuePair<string, string>(@".", ":doNothing"),
            });
    }
}
