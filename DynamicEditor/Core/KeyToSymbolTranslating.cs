using System.Collections.Generic;
using DynamicEditor.Core.Exceptions;

namespace DynamicEditor.Core
{
    internal class KeyToSymbolTranslating
    {
        private readonly Dictionary<string, string> _keyMap;

        public KeyToSymbolTranslating(Dictionary<string, string> keyMap)
        {
            _keyMap = keyMap;
        }

        public string GetSymbolFor(string input)
        {
            if (!_keyMap.ContainsKey(input))
                return "";

            return _keyMap[input];
        }
    }
}