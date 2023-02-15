using CUI.ColorSchemes;
using CUI.OutBuffers;
using System.Reflection;

namespace CUI;

public static class Settings
{
    public static IColorScheme ColorScheme { get; set; }
    public static IOutBuffer OutBuffer { get; set; }
    public static string ApplicationPath { get; set; }

    static Settings()
    {
        ColorScheme = new DefaultLightColorScheme();
        ApplicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
    }
}
