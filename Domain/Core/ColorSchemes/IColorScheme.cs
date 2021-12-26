namespace Domain.Core.ColorSchemes
{
    public interface IColorScheme
    {
        string Name { get; init; }

        string TextForeground { get; init; }
        string TextBackground { get; init; }
        string TextSelectionForeground { get; init; }
        string TextSelectionBackground { get; init; }

        string CursorForeground { get; init; }
        string CursorBackground { get; init; }

        string DeveloperMonitorForeground { get; init; }
        string DeveloperMonitorBackground { get; init; }

        string WidgetBorderForeground { get; init; }
        string WidgetBorderBackground { get; init; }
        string WidgetActiveButtonBackground { get; init; }
        string WidgetActiveButtonForeground { get; init; }
        string WidgetInactiveButtonBackground { get; init; }
        string WidgetInactiveButtonForeground { get; init; }

        string AlertNoticeForeground { get; init; }
        string AlertWarnForeground { get; init; }
        string AlertErrorForeground { get; init; }
    }
}