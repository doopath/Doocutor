module Libraries.Core.OutputColorizing


open System

let defaultForegroundColor = ConsoleColor.White

let setDefaultColors() =
    Console.ForegroundColor <- defaultForegroundColor
    ()

let colorizeForeground (color: ConsoleColor) (fn: Action) =
    Console.ForegroundColor <- color
    fn.Invoke()
    setDefaultColors()