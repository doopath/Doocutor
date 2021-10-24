namespace Domain.Core.ColorSchemes
{
    public record DefaultDarkColorScheme : IColorScheme
    {
        public string Name { get; init; }
        public string TextForeground { get; init; }
        public string TextBackground { get; init; }
        public string CursorForeground { get; init; }
        public string CursorBackground { get; init; }
        public string DeveloperMonitorForeground { get; init; }
        public string DeveloperMonitorBackground { get; init; }

        public DefaultDarkColorScheme()
        {
            Name = "DoocutorDark";
            TextForeground = "#eeeeee";
            TextBackground = "#555555";
            CursorForeground = "#555555";
            CursorBackground = "#eeeeee";
            DeveloperMonitorForeground = "#eeeeee";
            DeveloperMonitorBackground = "#ae8abe";
        }
    }
}