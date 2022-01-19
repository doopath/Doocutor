using CommandLine;

namespace Common.Options;

public sealed record AppOptions
{
    [Option('c', "color-scheme", Required = false,
        Default = "DoocutorDark", HelpText = "Color scheme name")]
    public string? ColorScheme { get; set; } = "DoocutorDark";


    [Option("dev-monitor", Required = false,
        Default = false, HelpText = "Enable/Disable developer monitor")]
    public bool IsDeveloperMonitorEnabled { get; set; } = false;


    [Option("out-buffer-size-update-rate", Required = false, Default = 300,
        HelpText = "How fast OutBufferSizeHandler refreshes the screen when you resize the console.")]
    public int OutBufferSizeHandlerUpdateRate { get; set; } = 300;


    [Option("history-limit", Required = false, Default = 10000,
        HelpText = "How many done changes are stored in a history (for undo/redo options).")]
    public int TextBufferHistoryLimit { get; set; } = 10000;


    [Option('f', "file", Required = false, Default = null,
        HelpText = "Open Doocutor with that file.")]
    public string? TextBufferPath { get; set; } = null;

    [Option("tab-symbol", Required = false, Default = " ",
        HelpText = "I use spaces, btw")]
    public string TabSymbol { get; set; } = " ";

    [Option("tab-width", Required = false, Default = 4,
        HelpText = "Width of the tab.")]
    public int TabWidth { get; set; } = 4;
}
