using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doocutor.Core
{
    static class NativeCommandsList
    {
        private static List<string> Commands = Enumerable.ToList<string>(new string[] {
            ":quit",
            ":compile",
            ":run",
            ":using",
            ":writeAfter",
            ":remove",
            ":removeBlock",
            ":replace"
        });

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
