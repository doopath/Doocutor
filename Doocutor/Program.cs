using System;
using NLog;
using Doocutor.Core;
using Doocutor.Core.Descriptors;
using Doocutor.Core.Exceptions;
using Libraries.Core;

using Info = Doocutor.Core.Info;

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
                ErrorHandling.handleInterruptedExecutionException(error, End);
            }
            catch (Exception error)
            {
                ErrorHandling.showError(error);
            }

            End();
        }

        private static void Start()
            => OutputColorizing.colorizeForeground(ConsoleColor.Cyan, () => {
                Logger.Debug("Start of the program");
                Info.ShowDoocutorInfo();
            });

        private static void End()
            => OutputColorizing.colorizeForeground(ConsoleColor.Cyan,
                () =>
                {
                    Logger.Debug("End of the program\n\n");
                    OutputColorizing.colorizeForeground(ConsoleColor.Cyan, () => Console.WriteLine("\nGood bye!\n"));
                });
    }
}
