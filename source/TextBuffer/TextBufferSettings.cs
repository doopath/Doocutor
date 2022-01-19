namespace TextBuffer;

public static class TextBufferSettings
{
    public static string Tab
    {
        get => new string(TabSymbol, TabWidth);
        set => TabSymbol = value[0];
    }

    public static int TabWidth { get; set; } = 4;
    private static char TabSymbol { get; set; } = ' ';
}