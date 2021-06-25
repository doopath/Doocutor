using System;
using Doocutor.Core.Exceptions;
using NLog;

namespace Doocutor.Core
{
    internal static class ErrorHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void HandleInterruptedExecutionException(InterruptedExecutionException error, Action endingFunction)
        {
            OutputColorizer.ColorizeForeground(ConsoleColor.Cyan, () => Logger.Debug(error.Message));
            endingFunction();
            Environment.Exit(0);
        }

        public static void ShowError(Exception error)
            => OutputColorizer.ColorizeForeground(ConsoleColor.Red, () => Logger.Error(error.Message));

        public static void ShowError(string error)
            => OutputColorizer.ColorizeForeground(ConsoleColor.Red, () => Logger.Error(error));
    }
}