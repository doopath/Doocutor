using CommandLine;

namespace Common.Options;

public sealed record AppOptions
{
    [Option('c', "color-scheme", Required = false,
        Default = "DoocutorDark", HelpText = "Color scheme name")]
    public string ColorScheme { get; set; }
    
    
    [Option("dev-monitor", Required = false,
        Default = false, HelpText = "Enable/Disable developer monitor")]
    public bool IsDeveloperMonitorEnabled { get; set; }
    
    
    [Option("out-buffer-size-update-rate", Required = false, Default = 300,
        HelpText = "How fast OutBufferSizeHandler refreshes the screen when you resize the console.")]
    public int OutBufferSizeHandlerUpdateRate { get; set; }
    
    
    [Option("history-limit", Required = false, Default = 10000,
        HelpText = "How many done changes are stored in a history (for undo/redo options).")]
    public int TextBufferHistoryLimit { get; set; }
}
