using System.Collections.Generic;

namespace Doocutor.Core
{
    internal static class NativeCommandsList
    {
        public static readonly List<string> Commands = NativeCommander.SupportedCommands;

        public static bool Contains(string command) => NativeCommander.SupportedCommands.Contains(command);
    }
}
