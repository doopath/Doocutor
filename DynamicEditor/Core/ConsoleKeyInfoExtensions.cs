using System;
using System.Linq;
using System.Collections.Generic;

namespace DynamicEditor.Core
{
    internal static class ConsoleKeyInfoExtensions
    {
        public static string ToKeyCombination(this ConsoleKeyInfo combination)
        {
            var modifiers = combination.GetModifiersList();

            modifiers.Reverse();
            modifiers.Add(combination.Key.ToString());

            return string.Join("+", modifiers);
        }

        public static List<string> GetModifiersList(this ConsoleKeyInfo combination)
        {
            var modifiers = combination.Modifiers.ToString().Split(", ").ToList();

            if (modifiers.Count == 1 && modifiers.Contains("0"))
                return new List<string>();
            else
                return modifiers;
        }
    }
}
