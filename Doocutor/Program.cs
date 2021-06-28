using System;
using NLog;
using Doocutor.Core;
using Doocutor.Core.Descriptors;
using Doocutor.Core.Exceptions;
using Libraries.Core;

using Info = Doocutor.Core.Info;
using ErrorHandler = Libraries.Core.ErrorHandler;

namespace Doocutor
{
    internal static class Program
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
                ErrorHandler.handleInterruptedExecutionException(error, End);
            }
            catch (Exception error)
            {
                ErrorHandler.showError(error);
            }

            End();
        }

        private static void Start()
            => OutputColorizer.colorizeForeground(ConsoleColor.Cyan, () => {
                Logger.Debug("Start of the program");
                Info.ShowDoocutorInfo();
            });

        private static void End()
            => OutputColorizer.colorizeForeground(ConsoleColor.Cyan,
                () =>
                {
                    Logger.Debug("End of the program\n\n");
                    OutputColorizer.colorizeForeground(ConsoleColor.Cyan, () => Console.WriteLine("\nGood bye!\n"));
                });
    }
}
