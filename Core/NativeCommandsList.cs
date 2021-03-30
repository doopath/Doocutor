using System.Collections.Generic;

namespace Doocutor.Core
{
    static class NativeCommandsList
    {
        private static readonly List<string> Commands = NativeCommander.SupprotedCommands;

        public static bool Contains(string command)
        {
            return Commands.Contains(command);
        }

        public static List<string> GetCommands()
        {
            return Commands;
        }
    }
}
