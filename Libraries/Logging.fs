namespace Libraries.Logging

open NLog

module Logging =
    let createConfig() = Config.LoggingConfiguration()
    
    let createConsoleTarget (name: string) = new Targets.ConsoleTarget(name)
    
    let createFileTarget (name: string) = new Targets.FileTarget(name)
    
    let createRule() = Config.LoggingRule()
    
    let addTarget =
        fun (r: Config.LoggingRule) ->
            fun (t: Targets.Target) ->
                r.Targets.Add t
                r
                
    let addRule =
        fun (c: Config.LoggingConfiguration) ->
            fun (r: Config.LoggingRule) ->
                c.LoggingRules.Add r
                c
   
    let enableLogging =
        fun (r: Config.LoggingRule) ->
            fun (l: LogLevel) ->
                r.EnableLoggingForLevel l
                r
                
    let setConfiguration (c: Config.LoggingConfiguration) = LogManager.Configuration <- c
    
    let getLogger name = LogManager.GetLogger name
                
     
    let getDefaultLogger name =
        let consoleTarget = createConsoleTarget "ConsoleTarget"
        let fileTarget = createFileTarget "FileName"
        
        let rule =
            createRule()
            |> (fun r -> enableLogging r LogLevel.Info)
            |> (fun r -> addTarget r consoleTarget)
            |> (fun r -> addTarget r fileTarget)
            
        let config = addRule (createConfig()) rule
        
        setConfiguration config
        getLogger name