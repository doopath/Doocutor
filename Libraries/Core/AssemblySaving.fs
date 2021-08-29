module Libraries.Core.AssemblySaving


open System.IO
open Libraries.Core.FileSystem

let notifyAboutWrongFileExtension() =
    printfn "By default doocutor makes an assemblies as a dynamic links library, so your file fill be overridden with \".dll\" extension."

let saveAssembly (path: string) assembly =
    let saveWithChangedExtension() =
        notifyAboutWrongFileExtension()
        File.WriteAllBytes(getGlobalPath (changeFileExtension "dll" path), assembly)

    match isDll path with
    | true -> File.WriteAllBytes(getGlobalPath path, assembly)
    | false -> saveWithChangedExtension()