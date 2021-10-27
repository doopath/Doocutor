namespace Domain.Core.ColorSchemes
{
    public interface IColorScheme
    {
        string Name { get; init; }
        string TextForeground { get; init; }
        string TextBackground { get; init; }
        string CursorForeground { get; init; }
        string CursorBackground { get; init; }
        string DeveloperMonitorForeground { get; init; }
        string DeveloperMonitorBackground { get; init; }
    }
}