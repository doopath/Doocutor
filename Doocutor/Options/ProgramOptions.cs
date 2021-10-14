using System.ComponentModel.DataAnnotations;
using CommandLine;

namespace Doocutor.Options
{
    internal sealed record ProgramOptions
    {
        [Option('m', "editor-mode", Required = false, Default = "Static", HelpText = "Editor mode: dynamic/static")]
        public string EditorMode { get; set; }
    }
}
