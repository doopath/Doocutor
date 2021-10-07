using CommandLine;
using System.ComponentModel;

namespace Doocutor.Options
{
    internal sealed record ProgramOptions
    {
        [Description("Editor mode: dynamic/static")]
        [Option('m', "editor-mode", Required = false, Default = "Static")]
        public string EditorMode { get; init; }
    }
}
