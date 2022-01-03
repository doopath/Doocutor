namespace CUI.ColorSchemes;

public record DefaultDarkColorScheme : IColorScheme
{
    public string Name { get; init; }

    #region Text
    public string TextForeground { get; init; }
    public string TextBackground { get; init; }
    public string TextSelectionForeground { get; init; }        
    public string TextSelectionBackground { get; init; }
    #endregion

    #region Cursor
    public string CursorForeground { get; init; }
    public string CursorBackground { get; init; }
    #endregion

    #region Developer monitor
    public string DeveloperMonitorForeground { get; init; }
    public string DeveloperMonitorBackground { get; init; }
    #endregion

    #region Widget
    public string WidgetBorderForeground { get; init; }
    public string WidgetBorderBackground { get; init; }
    public string WidgetActiveButtonForeground { get; init; }
    public string WidgetActiveButtonBackground { get; init; }
    public string WidgetInactiveButtonBackground { get; init; }
    public string WidgetInactiveButtonForeground { get; init; }
    #endregion

    #region Alert
    public string AlertNoticeForeground { get; init; }
    public string AlertWarnForeground { get; init; }
    public string AlertErrorForeground { get; init; }
    #endregion

    public DefaultDarkColorScheme()
    {
        Name = "DoocutorDark";

        TextForeground = "#A9B7C6";
        TextBackground = "";
        TextSelectionForeground = "#2B2B2B";
        TextSelectionBackground = "#A9B7C6";

        CursorForeground = "#2B2B2B";
        CursorBackground = "#A9B7C6";

        DeveloperMonitorForeground = "#EEEEEE";
        DeveloperMonitorBackground = "#9876AA";

        WidgetBorderForeground = "#EEEEEE";
        WidgetBorderBackground = "";
        WidgetActiveButtonForeground = "#2B2B2B";
        WidgetActiveButtonBackground = "#EEEEEE";
        WidgetInactiveButtonBackground = "#A9B7C6";
        WidgetInactiveButtonForeground = "#EEEEEE";

        AlertNoticeForeground = TextForeground;
        AlertWarnForeground = "#FE8019";
        AlertErrorForeground = "#FB4934";
    }
}
