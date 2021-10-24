using CommandLine;

namespace Domain.Options
{
    public sealed record ProgramOptions
    {
        [Option('m', "editor-mode", Required = false,
            Default = "Static", HelpText = "Editor mode: dynamic/static")]
        public string? EditorMode { get; set; }
        
        [Option('c', "color-scheme", Required = false,
            Default = "DoocutorDark", HelpText = "Color scheme name")]
        public string? ColorScheme { get; set; }
    }
}
