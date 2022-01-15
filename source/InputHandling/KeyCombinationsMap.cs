namespace InputHandling
{
    public static class KeyCombinationsMap
    {
        public static Dictionary<string, string> Map { get; } = new(
            new[] {
                new KeyValuePair<string, string>(@"Alt\+Q", ":quit"),
                
                new KeyValuePair<string, string>(@"Alt\+U", ":undo"),
                new KeyValuePair<string, string>(@"Alt\+R", ":redo"),
                
                new KeyValuePair<string, string>(@"Alt\+W", ":saveCurrentBuffer"),
                new KeyValuePair<string, string>(@"Alt\+O", ":openTextBuffer"),
                
                new KeyValuePair<string, string>(@"Alt\+P", ":pasteClipboardContent"),
                new KeyValuePair<string, string>(@"Alt\+S", ":selectLines"),
                
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
