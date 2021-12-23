using System;
using System.Collections.Generic;
using Pastel;

namespace Domain.Core.Widgets;

public sealed class AlertWidget : Widget
{
    public AlertWidget(string text, AlertLevel level = AlertLevel.NOTICE)
    {
        _textForegroundColor = level switch
        {
            AlertLevel.NOTICE => ColorScheme!.AlertNoticeForeground,
            AlertLevel.WARN => ColorScheme!.AlertWarnForeground,
            AlertLevel.ERROR => ColorScheme!.AlertErrorForeground,
            _ => throw new ArgumentException($"{nameof(level)} is unknown!")
        };
        _horizontalSymbol = "─".Pastel(ColorScheme.WidgetBorderForeground);
        _verticalSymbol = "│".Pastel(ColorScheme.WidgetBorderForeground);
        _topLeftCorner = "╭".Pastel(ColorScheme.WidgetBorderForeground);
        _topRightCorner = "╮".Pastel(ColorScheme.WidgetBorderForeground);
        _bottomLeftCorner = "╰".Pastel(ColorScheme.WidgetBorderForeground);
        _bottomRightCorner = "╯".Pastel(ColorScheme.WidgetBorderForeground);
        _activeButtonIndex = 0;

        UpdateWidth();
        UpdateHeight();

        Text = text;
        Items = new();

        Refresh();
    }

    public override IEnumerator<string> GetEnumerator()
        => new AlertWidgetEnumerator(Lines);
}

internal class AlertWidgetEnumerator : WidgetEnumerator
{
    public AlertWidgetEnumerator(IEnumerable<string> lines) : base(lines) { }
}

public enum AlertLevel
{
    NOTICE,
    WARN,
    ERROR
}