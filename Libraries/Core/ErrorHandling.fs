module Libraries.Core.ErrorHandling


open System
open Libraries.Core.OutputColorizing

open NLog

let consoleLogger = LogManager.GetLogger "ErrorHandlerConsoleLogger"
let fileLogger = LogManager.GetLogger "ErrorHandlerFileLogger"

let showError (exc: Exception) =
    colorizeForeground ConsoleColor.Red (Action (fun () -> 
        consoleLogger.Error (exc.Message)
        fileLogger.Debug ($"Thrown an error '%s{exc.Message}'" + exc.StackTrace + "\n")))

let handleInterruptedExecutionException (exc: Exception) (fn: Action) =
    colorizeForeground ConsoleColor.Cyan (Action (fun () -> fileLogger.Debug exc.Message))
    fn.Invoke()
    Environment.Exit(0)