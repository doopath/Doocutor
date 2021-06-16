namespace DoocutorLibraries.Core

open System
open NLog

module Common =
    let uppercaseFirstLetter (s: string) =
        if s.Length < 1 then
            failwith "Cannot uppercase first symbol of an empty string!"

        s.ToCharArray().[0].ToString().ToUpper() + String.Join("", s.ToCharArray().[1..])


module OutputColorizer =
    let defaultForegroundColor = ConsoleColor.White

    let setDefaultColors() =
        Console.ForegroundColor <- defaultForegroundColor
        ()

    let colorizeForeground = fun c -> fun (f: Action) ->
        Console.ForegroundColor <- c
        f.Invoke()
        setDefaultColors()


module ErrorHandler =
    let logger = LogManager.GetLogger "ErrorHandler"

    let showError (e: Exception) =
        OutputColorizer.colorizeForeground ConsoleColor.Red (Action (fun () -> logger.Error e))

    let showErrorMessage (e: Exception) =
        OutputColorizer.colorizeForeground ConsoleColor.Red (Action (fun () -> logger.Error e.Message))

    let handleInterruptedExecutionException = fun (e: Exception) -> fun (f: Action) ->
        OutputColorizer.colorizeForeground ConsoleColor.Cyan (Action (fun () -> logger.Debug e.Message))
        f.Invoke()
        Environment.Exit(0)