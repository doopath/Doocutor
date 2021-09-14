using System;
using System.Collections.Generic;

namespace DynamicEditor.Core
{
    public static class KeyCombinationsMap
    {
        private static readonly Dictionary<string, string> _map = new(
            new[] {
                new KeyValuePair<string, string>("Alt+C", ":showPos")
            });

        public static Dictionary<string, string> Map => _map;
    }
}
