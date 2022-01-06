using Common;
using Pastel;

namespace CUI.Widgets;

public sealed class DeveloperMonitorWidget : Widget
{
    private readonly ITextBuffer _textBuffer;
    private readonly string _textBackgroundColor;

    private List<string> _monitor;
    private int TopOffset => CuiRender.TopOffset;
    private int PositionFromTop => _textBuffer.CursorPositionFromTop;
    private int PositionFromLeft => _textBuffer.CursorPositionFromLeft;
    private long RenderTime => CuiRender.LastFrameRenderTime;
    private int _renderedFrames;
    private long _renderTimeAcc;
    private int _avgRenderTime;
    private string? _longestMonitorLine;
    private const int AvgFramesCount = 5;

    public DeveloperMonitorWidget()
    {
        _monitor = new();
        _textBuffer = CuiRender.TextBuffer!;
        _renderedFrames = 0;
        _renderTimeAcc = 0;
        _avgRenderTime = 0;

        _textForegroundColor = Settings.ColorScheme.DeveloperMonitorForeground;
        _textBackgroundColor = Settings.ColorScheme.DeveloperMonitorBackground;

        _textLeftEdge = 2;
        _textRightEdge = 2;
        _textBottomEdge = 2;
        _horizontalSymbol = "─".Pastel(ColorScheme!.WidgetBorderForeground);
        _verticalSymbol = "│".Pastel(ColorScheme.WidgetBorderForeground);
        _topLeftCorner = "╭".Pastel(ColorScheme.WidgetBorderForeground);
        _topRightCorner = "╮".Pastel(ColorScheme.WidgetBorderForeground);
        _bottomLeftCorner = "╰".Pastel(ColorScheme.WidgetBorderForeground);
        _bottomRightCorner = "╯".Pastel(ColorScheme.WidgetBorderForeground);
        _activeButtonIndex = 0;

        Text = "";
        Items = new();

        Refresh();
    }

    public override IEnumerator<string> GetEnumerator()
        => new DeveloperMonitorWidgetEnumerator(Lines);
    
    public override void OnMounted(Action unmount, Action render)
        => render();
    
    protected override void Refresh()
    {
        Update();
        _monitor = GetMonitor().ToList();
        Text = string.Join("", _monitor);
        Items!.Clear();
        
        CursorPosition = new()
        {
            Left = CuiRender.RightEdge - _monitor[0].Length - 3,
            Top = CuiRender.TopEdge + 1
        };

        UpdateWidth();
        UpdateHeight();
        FillByEmptyItems();
        AddBorder();
        AddText();
    }
    
    protected override void UpdateWidth()
        => Width = _monitor[0].Length + 4;

    protected override void UpdateHeight()
        => Height = _monitor.Count + 2;

    protected override bool HandleInput()
        => false;

    private IEnumerable<string> GetMonitor()
    {
        List<string> monitor = new()
        {
            $"Top: [ offset: {TopOffset}; pos: {PositionFromTop} ]",
            $"Left: [ pos: {PositionFromLeft} ]",
            $"Render [ avg: {_avgRenderTime}ms; last: {RenderTime}ms ]"
        };

        _longestMonitorLine = monitor
            .OrderByDescending(l => l.Length)
            .ToArray()[0];

        string GetSpacesForShorterLine(string longestLine, string line)
            => new(' ', longestLine.Length - line.Length + 1);

        string GroupLine(string line)
            => line + GetSpacesForShorterLine(_longestMonitorLine, line);

        return monitor.Select(GroupLine);
    }
    
    public void Update()
    {
        _renderTimeAcc += RenderTime;
        _renderedFrames++;

        if (_renderedFrames == AvgFramesCount)
        {
            _avgRenderTime = (int)_renderTimeAcc / AvgFramesCount;
            _renderTimeAcc = 0;
            _renderedFrames = 0;
        }
    }
}

internal class DeveloperMonitorWidgetEnumerator : WidgetEnumerator
{
    public DeveloperMonitorWidgetEnumerator(IEnumerable<string> lines) : base(lines) { }
}