using System.Collections.Generic;
using Microsoft.FSharp.Collections;

namespace DynamicEditor.Core
{
    internal static class KeyMap
    {
        public static readonly Dictionary<string, string> Map = new(new[]
        {
            new KeyValuePair<string, string>("Spacebar", " "),
            new KeyValuePair<string, string>("Tab", "    "),
        });
    }
}