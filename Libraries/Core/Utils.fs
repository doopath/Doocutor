module Libraries.Core.Utils


let getFirst (l: 'a list) = l.[0]

let getSame i = i

let getFirstElOrEmptyString (l: string list) =
    match l.Length with
    | 0 -> ""
    | _ -> getFirst l
