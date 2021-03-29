using System;
using NLog;

using Doocutor.Core;
using Doocutor.Core.Descriptors;

namespace Doocutor
{
    internal class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            Logger.Info("Start of the program");

            try
            {
                IInputFlowDescriptor descriptor = new ConsoleInputFlowDescriptor();
                IInputFlowHandler handler = new CommandFlowHandler(descriptor);
                handler.Handle();
            }
            catch (Exception error)
            {
                Logger.Error(error);
            }

            Logger.Info("End of the program\n\n");
        }
    }
}
