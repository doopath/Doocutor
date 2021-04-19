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
        public const string BuildInfo = "UCB-080421";
        public const string Updated = "8th of April 2021";
        public const string Company = "Doopath";
        public const string Version = "0.1.0";
        public const string ProductName = "Doocutor";
        public const string ConfigurationAttribute = "Debug";

        public static string GetDoocutorInfo()
            => $"Doocutor v{Version}. Build: {BuildInfo}\n" +
               "Try :help command for getting help list, :info for getting doocutor description or :quit to exit.";
        
        public static void ShowDoocutorInfo() => Console.WriteLine(GetDoocutorInfo());
    }
}
