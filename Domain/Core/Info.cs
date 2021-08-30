using System;

namespace Domain.Core
{
    public static class Info
    {
        /// <summary>
        /// Build version variants:
        /// U - unstable
        /// S - stable
        /// F - full
        /// C - changes
        /// B - build
        /// DDMMYY - DayMonthYear datetime
        /// </summary>
        public const string BuildInfo = "UCB-300821";
        public const string Updated = "30th of August 2021";
        public const string Company = "Doopath";
        public const string Version = "0.2.2.1";
        public const string ProductName = "Doocutor";
        public const string BuildType = "Release";

        public static string DoocutorInfo
            => $"Doocutor v{Version}. Build: {BuildInfo}\n" +
               "Copyright (C) Doocutor 2021\n" +
               "Try :help command for help list, :info for a description of the Doocutor or :quit for quitting.";

        public static string Description
            => $"Doocutor v{Version}. Build: {BuildInfo}\n" +
               "Doocutor is doopath's project with GPL V3 license.\n" +
               "It's C# code executor (doo & executor) working in a terminal.";

        public static void ShowDoocutorInfo() => Console.WriteLine(DoocutorInfo);
    }
}
