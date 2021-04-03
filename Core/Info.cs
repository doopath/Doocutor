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
        public const string BuildInfo = "UCB-290321";
        public const string Updated = "30th of March 2021";
        public const string Company = "Doopath";
        public const string Version = "0.0.9";
        public const string ProductName = "Doocutor";
        public const string ConfigurationAttribute = "Debug";

        public static string GetDoocutorInfo()
            => $"Doocutor v{Version} with build: {BuildInfo}" +
               $"updated {Updated}.\nDeveloped in Russia.NizhnyNovgorod 2021.\n";
        
        public static void ShowDoocutorInfo() => Console.WriteLine(GetDoocutorInfo());
    }
}
