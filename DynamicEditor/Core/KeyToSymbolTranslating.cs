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

        public string? TryGetSymbolFor(string input)
        {
            try
            {
                return GetSymbolFor(input);
            }
            catch (ItemNotFoundException)
            {
                return null;
            }
        }

        public string GetSymbolFor(string input)
        {
            if (!_keyMap.ContainsKey(input))
                throw new ItemNotFoundException($"Symbol for key={input} was not found");

            return _keyMap[input];
        }
    }
}