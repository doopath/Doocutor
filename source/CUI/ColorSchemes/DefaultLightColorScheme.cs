namespace CUI.ColorSchemes
{
    public record DefaultLightColorScheme : IColorScheme
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

        #region Input Field
        public string InputFieldForeground { get; init; }
        public string InputFieldBackground { get; init; }
        public string InputFieldCursorForeground { get; init; }
        public string InputFieldCursorBackground { get; init; }
        #endregion
        
        public DefaultLightColorScheme()
        {
            Name = "DoocutorLight";

            TextForeground = "#555555";
            TextBackground = "";
            TextSelectionForeground = "#EEEEEE";
            TextSelectionBackground = "#555555";

            CursorForeground = "#EEEEEE";
            CursorBackground = "#555555";

            DeveloperMonitorForeground = "#555555";
            DeveloperMonitorBackground = "";

            WidgetBorderForeground = "#555555";
            WidgetBorderBackground = "";
            WidgetActiveButtonBackground = "#555555";
            WidgetActiveButtonForeground = "#EEEEEE";
            WidgetInactiveButtonBackground = "#A9B7C6";
            WidgetInactiveButtonForeground = "#EEEEEE";

            AlertNoticeForeground = TextForeground;
            AlertWarnForeground = "#AF3A03";
            AlertErrorForeground = "#9D0006";
            
            InputFieldForeground = "#EEEEEE";
            InputFieldBackground = "#555555";
            InputFieldCursorForeground = "#EEEEEE";
            InputFieldCursorBackground = "#A9B7C6";
        }
    }
}