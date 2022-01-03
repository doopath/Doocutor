namespace Common.Options;
using CommandLine;


public sealed record ProgramOptions
{
    [Option('c', "color-scheme", Required = false,
        Default = "DoocutorDark", HelpText = "Color scheme name")]
    public string? ColorScheme { get; set; }
}
