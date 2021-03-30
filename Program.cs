using System;
using NLog;

using Doocutor.Core;
using Doocutor.Core.Descriptors;
using Doocutor.Core.Exceptions;

namespace Doocutor
{
    internal class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            Start();

            try
            {
                IInputFlowDescriptor descriptor = new ConsoleInputFlowDescriptor();
                IInputFlowHandler handler = new CommandFlowHandler(descriptor);
                handler.Handle();
            }
            catch (InterruptedExecutionException error)
            {
                HandleInterruptedExecutionException(error);
            }
            catch (Exception error)
            {
                HandleAnyException(error);
            }

            End();
        }

        private static void Start()
        {
            Logger.Debug("Start of the program");
            Info.ShowDoocutorInfo();
        }

        private static void End()
        {
            Logger.Debug("End of the program\n\n");
        }

        private static void HandleInterruptedExecutionException(InterruptedExecutionException error)
        {
            Console.WriteLine(error.Message);
            Logger.Info(error.Message);
            End();
            Environment.Exit(0);
        }

        private static void HandleAnyException(Exception error)
        {
            Logger.Error(error);
        }
    }
}
