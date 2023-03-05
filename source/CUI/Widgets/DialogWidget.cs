using Pastel;

namespace CUI.Widgets;

public class DialogWidget : Widget
{
    protected virtual Action? OnCancel { get; set; }
    protected virtual Action<object?>? OnOk { get; set; }

    public DialogWidget(string text, Action? onCancel, Action<object?>? onOk)
    {
        _textLeftEdge = 2;
        _textRightEdge = 2;
        _textBottomEdge = 3;
        _textForegroundColor = ColorScheme!.TextForeground;
        _horizontalSymbol = "─".Pastel(ColorScheme.WidgetBorderForeground);
        _verticalSymbol = "│".Pastel(ColorScheme.WidgetBorderForeground);
        _topLeftCorner = "╭".Pastel(ColorScheme.WidgetBorderForeground);
        _topRightCorner = "╮".Pastel(ColorScheme.WidgetBorderForeground);
        _bottomLeftCorner = "╰".Pastel(ColorScheme.WidgetBorderForeground);
        _bottomRightCorner = "╯".Pastel(ColorScheme.WidgetBorderForeground);
        _activeButtonIndex = ButtonsMap.Keys.ToList().IndexOf(WidgetAction.CANCEL.ToString());

        OnCancel = onCancel;
        OnOk = onOk;

        UpdateWidth();
        UpdateHeight();

        Text = text;
        Items = new();
    }

    protected override Dictionary<string, WidgetAction> ButtonsMap { get; set; } = new(new[]
    {
        new KeyValuePair<string, WidgetAction>("OK", WidgetAction.OK),
        new KeyValuePair<string, WidgetAction>("CANCEL", WidgetAction.CANCEL),
    });

    protected override void HandleSelectedOption()
    {
        string activeButton = ButtonsMap.Keys.ToArray()[_activeButtonIndex];

        if (activeButton == "OK")
            OnOk?.Invoke(null);
        else if (activeButton == "CANCEL")
            OnCancel?.Invoke();
    }

    public override IEnumerator<string> GetEnumerator()
        => new DialogWidgetEnumerator(Lines);
}

internal class DialogWidgetEnumerator : WidgetEnumerator
{
    public DialogWidgetEnumerator(IEnumerable<string> lines) : base(lines) { }
}
