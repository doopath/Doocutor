using System;

namespace Domain.Core
{
    public static class Info
    {
        /// <summary>
        /// Build version variants:
        /// U / S - unstable / stable,
        /// F / C - full / changes,
        /// B - build,
        /// DDMMYY - DayMonthYear datetime
        /// </summary>
        public const string BuildInfo = "UCB-311021";
        public const string Updated = "31nd of October 2021";
        public const string Company = "Doopath";
        public const string Version = "0.5.5.1";
        public const string ProductName = "Doocutor";
        public const string BuildType = "Debug";

        public static string DoocutorInfo
            => $"Doocutor v{Version}. Build: {BuildInfo}\n" +
               "Copyright (C) Doocutor 2021\n" +
               "Try :help command for help list, :info for a description of the Doocutor or :quit for quitting.";

        public static string Description
            => $"Doocutor v{Version}. Build: {BuildInfo}.\n" +
               $"Last update: {Updated}.\n" +
               "It's a terminal text editor written in C# and F# by Doopath.";

        public static void ShowDoocutorInfo() => Console.WriteLine(DoocutorInfo);
    }
}
