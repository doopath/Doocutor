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
            TextForeground = "#A9B7C6";
            TextBackground = "#2B2B2B";
            CursorForeground = "#2B2B2B";
            CursorBackground = "#A9B7C6";
            DeveloperMonitorForeground = "#EEEEEE";
            DeveloperMonitorBackground = "#9876AA";
        }
    }
}