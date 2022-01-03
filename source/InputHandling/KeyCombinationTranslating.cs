namespace InputHandling
{
    public class KeyCombinationTranslating
    {
        private readonly Dictionary<string, string> _keyCombinationsMap;

        public KeyCombinationTranslating(Dictionary<string, string> keyCombinationsMap)
        {
            _keyCombinationsMap = keyCombinationsMap;
        }

        public string GetCommandFor(string keyCombination)
        {
            if (!_keyCombinationsMap.ContainsKey(keyCombination))
                return "";

            return _keyCombinationsMap[keyCombination];
        }
    }
}
