using System;
using NLog;

namespace Doocutor
{
    class Program
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            LOGGER.Info("Start of the program");
            Console.WriteLine("Hello World!");
            LOGGER.Info("End of the program");
        }
    }
}
