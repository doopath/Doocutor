namespace Libraries.Core

open System

module Utils =
    let getFirst (l: 'a list) = l.[0]

    let getFirstElOrEmptyString (l: string list) =
        match l.Length with
        | 0 -> ""
        | _ -> getFirst l


module Common =
    let uppercaseFirstLetter (s: string) =
        match s.Length with
        | 0 -> failwith "Cannot uppercase first symbol of an empty string!"
        | _ -> s.ToCharArray().[0].ToString().ToUpper() + String.Join("", s.ToCharArray().[1..])


module OutputColorizer =
    let defaultForegroundColor = ConsoleColor.White

    let setDefaultColors() =
        Console.ForegroundColor <- defaultForegroundColor
        ()

    let colorizeForeground (color: ConsoleColor) (fn: Action) =
        Console.ForegroundColor <- color
        fn.Invoke()
        setDefaultColors()


module ErrorHandler =
    open NLog

    let logger = LogManager.GetLogger "ErrorHandler"

    let showError (e: Exception) =
        OutputColorizer.colorizeForeground ConsoleColor.Red (Action (fun () -> logger.Error e))

    let showErrorMessage (e: Exception) =
        OutputColorizer.colorizeForeground ConsoleColor.Red (Action (fun () -> 
            logger.Error e.Message
            logger.Debug e))

    let handleInterruptedExecutionException (exc: Exception) (fn: Action) =
        OutputColorizer.colorizeForeground ConsoleColor.Cyan (Action (fun () -> logger.Debug exc.Message))
        fn.Invoke()
        Environment.Exit(0)


module FileSystem =
    open Utils

    let getHomeDir() =
        match (Environment.GetFolderPath Environment.SpecialFolder.UserProfile) with
        | null -> Environment.GetEnvironmentVariable "HOME" // "HOME" is a default global variable in Linux and OSX
        | winHomeDir -> winHomeDir // Default home global variable in Windows 10

    let isShortedPath (path: string) = path.Trim().StartsWith "~"

    let getGlobalPath (path: string) = 
        match isShortedPath path with
        | true -> getHomeDir() + path.[1..]
        | false -> path

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
