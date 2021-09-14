using System.Collections.Generic;
using Domain.Core.Commands;
using DynamicEditor.Core.Exceptions;

namespace DynamicEditor.Core
{
    internal class KeyCombinationTranslating
    {
        private readonly Dictionary<string, string> _keyCombinationsMap;

        public KeyCombinationTranslating(Dictionary<string, string> keyCombinationsMap)
        {
            _keyCombinationsMap = keyCombinationsMap;
        }

        public string? TryGetCommandFor(string keyCombination)
        {
            try
            {
                return GetCommandFor(keyCombination);
            }
            catch (ItemNotFoundException)
            {
                return null;
            }
        }

        public string GetCommandFor(string keyCombination)
        {
            if (!_keyCombinationsMap.ContainsKey(keyCombination))
                throw new ItemNotFoundException($"Command for key combination [{keyCombination}] not found");

            return _keyCombinationsMap[keyCombination];
        }
    }
}
