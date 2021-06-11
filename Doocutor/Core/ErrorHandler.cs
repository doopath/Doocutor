using System;
using Doocutor.Core.Exceptions;
using DoocutorLibraries.CoreLibrary;
using NLog;

namespace Doocutor.Core
{
    internal static class ErrorHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void HandleInterruptedExecutionException(InterruptedExecutionException error, Action endingFunction)
        {
            OutputColorizer.colorizeForeground(ConsoleColor.Cyan, () => Logger.Debug(error.Message));
            endingFunction();
            Environment.Exit(0);
        }

        public static void ShowError(Exception error)
            => OutputColorizer.colorizeForeground(ConsoleColor.Red, () => Logger.Error(error.Message));

        public static void ShowError(string error)
            => OutputColorizer.colorizeForeground(ConsoleColor.Red, () => Logger.Error(error));
    }
}