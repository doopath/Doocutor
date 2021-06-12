namespace DoocutorLibraries.Core

open System
open NLog
open DoocutorLibraries.Logging

module Common =
    type Error =
        | Exception of Exception
        | Message of string
        
    let uppercaseFirstLetter (s: string) =
        if s.Length < 1 then
            failwith "Cannot uppercase first symbol of an empty string!"
            
        s.ToCharArray().[0].ToString().ToUpper() + String.Join("", s.ToCharArray().[1..])


module Info = 
    let buildInfo = "UCB-100621"
    let updated = "10th of June 2021"
    let company = "Doopath"
    let version = "0.1.5"
    let configurationAttribute = "Debug"

    
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
    
    let showError (e: Common.Error) =
        OutputColorizer.colorizeForeground ConsoleColor.Red (Action (fun () -> logger.Error e.ToString))
        
    let handleInterruptedExecutionException = fun (e: Exception) -> fun (f: Action) ->
        OutputColorizer.colorizeForeground ConsoleColor.Cyan (Action (fun () -> logger.Debug e.Message))
        f.Invoke()
        Environment.Exit(0)

