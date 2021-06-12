using System;
using NLog;
using Doocutor.Core;
using Doocutor.Core.Descriptors;
using Doocutor.Core.Exceptions;
using DoocutorLibraries.Core;

using Info = Doocutor.Core.Info;
using ErrorHandler = DoocutorLibraries.Core.ErrorHandler;
using Error = DoocutorLibraries.Core.Common.Error;

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
            // catch (SourceCodeCompilationException error)
            // {
            //     Logger.Debug(error);
            // }
            catch (Exception error)
            {
                ErrorHandler.showError(Error.NewException(error));
            }

            End();
        }

        private static void Start()
            => OutputColorizer.colorizeForeground(ConsoleColor.Cyan, () => {
                Logger.Debug("Start of the program");
                Info.ShowDoocutorInfo();
            });

        private static void End()
            => OutputColorizer.colorizeForeground(ConsoleColor.Cyan, () => Logger.Debug("End of the program\n\n"));
    }
}
