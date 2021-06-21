namespace Libraries.Core

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


module SourceCodeSaver =
    open System.IO

    let getHomeDir() =
        match Environment.GetEnvironmentVariable "userdir" with
        | null -> Environment.GetEnvironmentVariable "HOME" // "HOME" is a default global variable in Linux and OSX
        | winHomeDir -> winHomeDir // "userdir" is a default home global variable in windows10

    let getGlobalPath (shortedPath: string) = getHomeDir() + shortedPath.[1..]

    let isShortedPath (path: string) = path.Trim().StartsWith "~"

    let save (path: string) code =
        match isShortedPath path with
        | true -> File.WriteAllLines(getGlobalPath path, code)
        | false -> File.WriteAllLines(path, code)

