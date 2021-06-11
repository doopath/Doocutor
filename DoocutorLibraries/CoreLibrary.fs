namespace DoocutorLibraries.CoreLibrary

open System

module Info = 
    let buildInfo = "UCB-100621"
    let updated = "10th of June 2021"
    let company = "Doopath"
    let version = "0.1.5"
    let configurationAttribute = "Debug"

    
module OutputColorizer =
    let setDefaultColors() =
        Console.ForegroundColor <- ConsoleColor.White
        Console.BackgroundColor <- ConsoleColor.Black
        ()
        
    let colorizeForeground (color, action: Action) =
        Console.ForegroundColor <- color
        action.Invoke()
        setDefaultColors()
         
    let colorizeBackground (color, action: Action) =
        Console.BackgroundColor <- color
        action.Invoke()
        setDefaultColors()