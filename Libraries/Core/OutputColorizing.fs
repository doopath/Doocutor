module Libraries.Core.OutputColorizing


open System

let colorizeForeground (color: ConsoleColor) (fn: Action) =
    Console.ForegroundColor <- color
    fn.Invoke()
    Console.ResetColor();