module Libraries.Core.FileSystem


open System

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
    String.Join("", path.ToCharArray()
        |> List.ofArray
        |> List.rev).StartsWith("lld.")

let changeFileExtension (target: string) (path: string) = removeFileExtension path + $".%s{target}"