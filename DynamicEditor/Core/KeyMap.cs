using System.Collections.Generic;
using Microsoft.FSharp.Collections;

namespace DynamicEditor.Core
{
    internal static class KeyMap
    {
        public static readonly Dictionary<string, string> Map = new(new[]
        {
            #region Text-Symbols
            new KeyValuePair<string, string>("Spacebar", " "),
            new KeyValuePair<string, string>("Tab", "    "),
            #endregion

            #region D-Symbols
            new KeyValuePair<string, string>("D1", @"1"),
            new KeyValuePair<string, string>("D2", @"2"),
            new KeyValuePair<string, string>("D3", @"3"),
            new KeyValuePair<string, string>("D4", @"4"),
            new KeyValuePair<string, string>("D5", @"5"),
            new KeyValuePair<string, string>("D6", @"6"),
            new KeyValuePair<string, string>("D7", @"7"),
            new KeyValuePair<string, string>("D8", @"8"),
            new KeyValuePair<string, string>("D9", @"9"),
            new KeyValuePair<string, string>("D0", @"0"),

            new KeyValuePair<string, string>("Shift+D1", @"!"),
            new KeyValuePair<string, string>("Shift+D2", @"@"),
            new KeyValuePair<string, string>("Shift+D3", @"#"),
            new KeyValuePair<string, string>("Shift+D4", @"$"),
            new KeyValuePair<string, string>("Shift+D5", @"%"),
            new KeyValuePair<string, string>("Shift+D6", @"^"),
            new KeyValuePair<string, string>("Shift+D7", @"&"),
            new KeyValuePair<string, string>("Shift+D8", @"*"),
            new KeyValuePair<string, string>("Shift+D9", @"("),
            new KeyValuePair<string, string>("Shift+D0", @")"),
            #endregion

            #region OEM-Symbols
            new KeyValuePair<string, string>("Oem1", @";"),
            new KeyValuePair<string, string>("Oem2", @"/"),
            new KeyValuePair<string, string>("Oem3", @"`"),
            new KeyValuePair<string, string>("Oem4", @"["),
            new KeyValuePair<string, string>("Oem5", @"\"),
            new KeyValuePair<string, string>("Oem6", @"]"),
            new KeyValuePair<string, string>("Oem7", @"'"),
            new KeyValuePair<string, string>("OemComma", @","),
            new KeyValuePair<string, string>("OemMinus", @"-"),
            new KeyValuePair<string, string>("OemPeriod", @"."),
            new KeyValuePair<string, string>("OemPlus", @"="),

            new KeyValuePair<string, string>("Shift+Oem1", @":"),
            new KeyValuePair<string, string>("Shift+Oem2", @"?"),
            new KeyValuePair<string, string>("Shift+Oem3", @"~"),
            new KeyValuePair<string, string>("Shift+Oem4", @"{"),
            new KeyValuePair<string, string>("Shift+Oem5", @"|"),
            new KeyValuePair<string, string>("Shift+Oem6", @"}"),
            new KeyValuePair<string, string>("Shift+Oem7", "\""),
            new KeyValuePair<string, string>("Shift+OemComma", @"<"),
            new KeyValuePair<string, string>("Shift+OemPeriod", @">"),
            new KeyValuePair<string, string>("Shift+OemMinus", @"_"),
            new KeyValuePair<string, string>("Shift+OemPlus", @"+"),
            #endregion
        });
    }
}