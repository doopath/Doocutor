using System;

namespace Doocutor.Core
{
    internal static class Info
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
        public const string BuildInfo = "UCB-280621";
        public const string Updated = "28th of June 2021";
        public const string Company = "Doopath";
        public const string Version = "0.2.1.1";
        public const string ProductName = "Doocutor";
        public const string BuildType = "Release";

        public static string DoocutorInfo
            => $"Doocutor v{Version}. Build: {BuildInfo}\n" +
               "Try :help command for getting help list, :info for getting doocutor description or :quit to exit.";

        public static string Description
            => $"Doocutor v{Version}. Build: {BuildInfo}\n" +
               "Doocutor is doopath's project with GPL V3 license.\n" +
               "It's C# code executor (doo & executor) working in a terminal.";

        public static void ShowDoocutorInfo() => Console.WriteLine(DoocutorInfo);
    }
}
