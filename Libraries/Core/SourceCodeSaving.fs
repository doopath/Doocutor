module Libraries.Core.SourceCodeSaving


open System.IO
open Libraries.Core.FileSystem

let saveCode (path: string) code = File.WriteAllLines(getGlobalPath path, code)