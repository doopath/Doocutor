namespace Domain.Core.ColorSchemes
{
    public record DefaultLightColorScheme : IColorScheme
    {
        public string Name { get; init; }
        public string TextForeground { get; init; }
        public string TextBackground { get; init; }
        public string CursorForeground { get; init; }
        public string CursorBackground { get; init; }
        public string DeveloperMonitorForeground { get; init; }
        public string DeveloperMonitorBackground { get; init; }

        public DefaultLightColorScheme()
        {
            Name = "DoocutorLight";
            TextForeground = "#555555";
            TextBackground = "#eeeeee";
            CursorForeground = "#eeeeee";
            CursorBackground = "#555555";
            DeveloperMonitorForeground = "#eeeeee";
            DeveloperMonitorBackground = "#ae8abe";
        }
    }
}