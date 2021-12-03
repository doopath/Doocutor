using System;
using System.Diagnostics.CodeAnalysis;
using Domain.Core.Exceptions;
using Domain.Core.Widgets;
using NLog;

namespace Domain.Core;

public class ErrorHandling
{
    public static Logger FileLogger { get; }
    public static Logger ConsoleLogger { get; }

    static ErrorHandling()
    {
        FileLogger = LogManager.GetLogger("ErrorHandlerFileLogger");
        ConsoleLogger = LogManager.GetLogger("ErrorHandlerConsoleLogger");
    }

    public static void ShowError([NotNull] Exception error)
    => OutputColorizing.ColorizeForeground(ConsoleColor.Red, () =>
   {
       string alertMessage = $"ERROR: {error.Message}";
       WidgetsMount.Mount(new AlertWidget(alertMessage, AlertLevel.ERROR));
       FileLogger.Debug($"Thrown an error: \"{error.Message}\"\n {error.StackTrace} \n");
   });

    public static void HandleInterruptedExecutionException(
        [NotNull] InterruptedExecutionException error,
        [NotNull] Action action)
    {
        OutputColorizing.ColorizeForeground(ConsoleColor.Cyan, () => FileLogger.Debug(error.Message));
        action.Invoke();
        Environment.Exit(0);
    }
}