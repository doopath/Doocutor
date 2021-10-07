module Libraries.Core.Common


open System

let uppercaseFirstLetter (s: string) =
    match s.Length with
    | 0 -> failwith "Cannot uppercase first symbol of an empty string!"
    | _ -> s.ToCharArray().[0].ToString().ToUpper() + String.Join("", s.ToCharArray().[1..])