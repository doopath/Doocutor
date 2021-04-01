using System.Collections.Generic;

namespace Doocutor.Core
{
    static class NativeCommandsList
    {
        public static readonly List<string> Commands = NativeCommander.SupprotedCommands;

        public static bool Contains(string command) => NativeCommander.SupprotedCommands.Contains(command);
    }
}
