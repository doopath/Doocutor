using System.IO;
using System.Reflection;
using Domain.Core.ColorSchemes;
using Domain.Core.OutBuffers;

namespace Domain.Core;

public static class Settings
{
    public static IColorScheme ColorScheme { get; set; }
    public static IOutBuffer OutBuffer { get; set; }
    public static string ApplicationPath { get; }

    static Settings()
    {
        ColorScheme = new DefaultLightColorScheme();
        OutBuffer = new StandardConsoleOutBuffer();
        ApplicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
    }
}
