using System.Collections.Generic;
using Pastel;

namespace Domain.Core.Widgets;

public class AlertWidget : Widget, IWidget
{
    public AlertWidget(string text)
    {
        _borderColor = "#5E81AC";
        _horizontalSymbol = "─".Pastel(_borderColor);
        _verticalSymbol = "│".Pastel(_borderColor);
        _topLeftCorner = "╭".Pastel(_borderColor);
        _topRightCorner = "╮".Pastel(_borderColor);
        _bottomLeftCorner = "╰".Pastel(_borderColor);
        _bottomRightCorner = "╯".Pastel(_borderColor);
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
    public AlertWidgetEnumerator(IEnumerable<string> lines) : base(lines) {}
}

