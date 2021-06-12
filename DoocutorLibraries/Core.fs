namespace DoocutorLibraries.Core

open System

module Info = 
    let buildInfo = "UCB-100621"
    let updated = "10th of June 2021"
    let company = "Doopath"
    let version = "0.1.5"
    let configurationAttribute = "Debug"

    
module OutputColorizer =
    let defaultForegroundColor = ConsoleColor.White
    
    let setDefaultColors() =
        Console.ForegroundColor <- defaultForegroundColor
        ()
        
    let colorizeForeground (color, action: Action) =
        Console.ForegroundColor <- color
        action.Invoke()
        setDefaultColors()