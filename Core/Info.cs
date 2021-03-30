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
        /// DDMMYY - Day.Month.Year datetime
        /// </summary>
        public static readonly string BuildInfo = "UCB-290321";
        public static readonly string Updated = "30th of March 2021";
        public static readonly string Version = "0.0.9";

        public static void ShowDoocutorInfo() => Console.WriteLine($"Doocutor v{Version} with build: {BuildInfo}" +
            $"updated {Updated}.\nDeveloped in Russia.NizhnyNovgorod 2021.\n");
    }
}
