namespace Libraries.Core

open System
open NLog

module Utils =
    let getFirst (l: 'a list) = l.[0]

    let getFirstOrEmptyString (l: string list) =
        match l.Length with
        | 0 -> ""
        | _ -> getFirst l


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


module FileSystem =
    open Utils

    let getHomeDir() =
        match (Environment.GetFolderPath Environment.SpecialFolder.UserProfile) with
        | null -> Environment.GetEnvironmentVariable "HOME" // "HOME" is a default global variable in Linux and OSX
        | winHomeDir -> winHomeDir // "userdir" is a default home global variable in windows10

    let isShortedPath (path: string) = path.Trim().StartsWith "~"

    let removeFileExtension (path: string) =
        let joinWithPoint (l: string list) = String.Join(".", l)

        let reversedSplitPath =
            path.Split(".")
                |> List.ofArray
                |> List.rev

        match reversedSplitPath.Length with
        | 0 | 1 -> path
        | _ ->  reversedSplitPath |> List.tail |> List.rev |> joinWithPoint

    let isDll (path: string) =
        (path.ToCharArray()
            |> List.ofArray
            |> List.rev
            |> string).StartsWith("lld.")

    let changeFileExtension (target: string) (path: string) = removeFileExtension path + $".%s{target}"


module SourceCodeSaver =
    open System.IO
    open FileSystem

    let getGlobalPath (path: string) = 
        match isShortedPath path with
        | true -> getHomeDir() + path.[1..]
        | false -> path

    let saveCode (path: string) code = File.WriteAllLines(getGlobalPath path, code)


module AssemblySaver =
    open System.IO
    open SourceCodeSaver
    open FileSystem

    let notifyAboutWrongFileExtension() =
        printfn "By default doocutor makes an assemblies as a dynamic links library, so your file fill be overridden with \".dll\" extension."

    let saveAssembly (path: string) assembly =
        let saveWithChangedExtension() =
            notifyAboutWrongFileExtension()
            File.WriteAllBytes(getGlobalPath (changeFileExtension "dll" path), assembly)

        match isDll path with
        | true -> File.WriteAllBytes(getGlobalPath path, assembly)
        | false -> saveWithChangedExtension()
