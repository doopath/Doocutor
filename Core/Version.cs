using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static readonly string Updated = "29th of March 2021";
        public static readonly string Version = "0.0.9";
    }
}
