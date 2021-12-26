using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.TextBuffers;
using Pastel;
using TextCopy;

namespace Domain.Core.Widgets;

public delegate int GetAnIntegerDelegate();

public sealed class LinesSelectionWidget : Widget
{
    protected override Dictionary<string, WidgetAction> ButtonsMap { get; set; } =
        new(System.Array.Empty<KeyValuePair<string, WidgetAction>>());
    private ITextBuffer TextBuffer { get; init; }

    private readonly string _textBackgroundColor;

    private ChangingRange _linesSelectionRange;
    private int _longestLineLength;

    public LinesSelectionWidget(ITextBuffer textBuffer)
    {
        _textLeftEdge = 1;
        _textRightEdge = 1;
        _textBottomEdge = 1;
        _textForegroundColor = ColorScheme.TextSelectionForeground;
        _textBackgroundColor = ColorScheme.TextSelectionBackground;
        _horizontalSymbol = "─".Pastel(ColorScheme.WidgetBorderForeground);
        _verticalSymbol = "│".Pastel(ColorScheme.WidgetBorderForeground);
        _topLeftCorner = "╭".Pastel(ColorScheme.WidgetBorderForeground);
        _topRightCorner = "╮".Pastel(ColorScheme.WidgetBorderForeground);
        _bottomLeftCorner = "╰".Pastel(ColorScheme.WidgetBorderForeground);
        _bottomRightCorner = "╯".Pastel(ColorScheme.WidgetBorderForeground);
        _activeButtonIndex = 0;

        TextBuffer = textBuffer;

        _linesSelectionRange = new() {
            Start = TextBuffer.CursorPositionFromTop, 
            End = TextBuffer.CursorPositionFromTop + 1
        };

        UpdateWidth();
        UpdateHeight();

        Text = string.Empty;
        Items = new();

        Refresh();
    }
    
    protected override void Refresh()
    {
        CursorPosition = new()
        {
            Left = TextBuffer.GetPrefixLength(),
            Top = Math.Min(_linesSelectionRange.Start, _linesSelectionRange.End)
        };
        Text = string.Join("", TextBuffer.Lines[_linesSelectionRange.Start.._linesSelectionRange.End]);

        Items!.Clear();
        UpdateWidth();
        UpdateHeight();
        FillByEmptyItems();
        AddText();
    }
    
    protected override void UpdateWidth()
        => Width = _longestLineLength;

    protected override void UpdateHeight()
        => Height = Math.Abs(_linesSelectionRange.Start - _linesSelectionRange.End);
    
    public override void OnSceneUpdated(List<string> scene)
    {
        Refresh();

        int topPos = CursorPosition.Top;
        int leftPos = CursorPosition.Left;

        foreach (var line in this)
        {
            string sceneLine = scene[topPos];
            scene[topPos] = sceneLine[..leftPos] + line + sceneLine[(leftPos + Width)..];
            topPos++;
        }
    }
    
    protected override void AddText()
    {
        string[] textItems = Text!
            .ToCharArray()
            .Select(c => c
                .ToString()
                .Pastel(_textForegroundColor)
                .PastelBg(_textBackgroundColor))
            .ToArray();
        int leftEdge = _textLeftEdge!.Value;
        int rightEdge = Width - _textRightEdge!.Value;
        int bottomEdge = Height - _textBottomEdge!.Value;
        int widgetItemsTopPointer = 1;
        int maxLineLength = rightEdge - leftEdge;

        var IsAdded = () => widgetItemsTopPointer - 1 >= bottomEdge;
        var RemoveAllFromCurrentLine = () => Items![widgetItemsTopPointer]
            .RemoveRange(leftEdge, maxLineLength);
        var InsertSlicedToIndex = (int index) => Items![widgetItemsTopPointer]
            .InsertRange(leftEdge, textItems[..index]);

        while (textItems.Length > 0)
        {
            if (IsAdded())
                return;

            RemoveAllFromCurrentLine();

            if (textItems.Length > maxLineLength)
            {
                InsertSlicedToIndex(maxLineLength);
                textItems = textItems[maxLineLength..];
            }
            else
            {
                InsertSlicedToIndex(textItems.Length);
                Items![widgetItemsTopPointer].InsertRange(
                    leftEdge + textItems.Length,
                    new string(' ', maxLineLength - textItems.Length).Split(""));
                textItems = textItems[textItems.Length..];
            }

            widgetItemsTopPointer++;
        }
    }
    
    protected override bool ControlButtons()
    {
        ConsoleKeyInfo key = Settings.OutBuffer!.ReadKey();

        if (key.Key == ConsoleKey.DownArrow)
            _linesSelectionRange.End--;
        else if (key.Key == ConsoleKey.UpArrow)
            _linesSelectionRange.End++;
        else if (key.Key == ConsoleKey.Enter)
            return true;

        int buttonsCount = ButtonsMap.Keys.Count;
        _activeButtonIndex = ((_activeButtonIndex % buttonsCount)
                              + buttonsCount)
                             % buttonsCount;

        return false;
    }
    
    protected override void HandleSelectedOption()
    {
        string[] lines = TextBuffer.Lines;
        string[] selectedLines = lines[_linesSelectionRange.Start.._linesSelectionRange.End];
        string text = string.Join("", selectedLines);
        
        new Clipboard().SetText(text);
    }
}

internal struct ChangingRange
{
    public int Start { get; set; }
    public int End { get; set; }
}