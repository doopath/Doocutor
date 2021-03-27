using System;
using NLog;

using Doocutor.Core;

namespace Doocutor
{
    class Program
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            LOGGER.Info("Start of the program");

            try
            {
                InputFlowDescriptor descriptor = new ConsoleInputFlowDescriptor();
                IInputFlowHandler handler = new CommandFlowHandler(descriptor);
                handler.Handle();
            }
            catch (Exception error)
            {
                LOGGER.Error(error);
            }

            LOGGER.Info("End of the program\n\n");
        }
    }
}
