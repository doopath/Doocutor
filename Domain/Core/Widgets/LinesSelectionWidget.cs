using System;
using System.Collections.Generic;
using Domain.Core.TextBuffers;
using Domain.Core.TextBuffers.TextPointers;
using Pastel;
using TextCopy;

namespace Domain.Core.Widgets;

public sealed class LinesSelectionWidget : Widget
{
    protected override Dictionary<string, WidgetAction> ButtonsMap { get; set; } =
        new(System.Array.Empty<KeyValuePair<string, WidgetAction>>());
    private ITextBuffer TextBuffer { get; init; }

    private readonly string _textBackgroundColor;
    private ConsoleKey? _lastPressedKey;
    private ChangingRange _linesSelectionRange;

    public LinesSelectionWidget(ITextBuffer textBuffer)
    {
        _textLeftEdge = 1;
        _textRightEdge = 1;
        _textBottomEdge = 1;
        _textForegroundColor = ColorScheme!.TextSelectionForeground;
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
            End = TextBuffer.CursorPositionFromTop
        };

        Width = 0;
        Height = 0;
        Text = string.Empty;
        Items = new();

        Refresh();
    }
    
    protected override void Refresh()
        => CursorPosition = new()
        {
            Left = TextBuffer.GetPrefixLength(),
            Top = Math.Min(_linesSelectionRange.Start, _linesSelectionRange.End)
        };

    public override void OnSceneUpdated(List<string> scene)
    {
        Refresh();

        int start = GetStartOfSelection();
        int end = GetEndOfSelection();
        
        start = start >= 0 ? start : 0;
        end = end >= scene.Count ? scene.Count - 1 : end;

        AddToScene(start, end, scene);
    }

    private void AddToScene(int start, int end, List<string> scene)
    {
        int prefixLength = TextBuffer.GetPrefixLength();

        for (int i = start; i <= end; i++)
        {
            string sceneLine = scene[i];
            string textBufferLine = TextBuffer.Size > i
                ? TextBuffer.Lines[i + CuiRender.TopOffset]
                : "";
            scene[i] = sceneLine[..(prefixLength - 1)]
                       + ("|" + textBufferLine)
                           .Pastel(_textForegroundColor)
                           .PastelBg(_textBackgroundColor)
                       + sceneLine[(prefixLength + textBufferLine.Length)..];
        }
    }

    private int GetStartOfSelection()
        => Math.Min(_linesSelectionRange.Start, _linesSelectionRange.End) - CuiRender.TopOffset;
    
    private int GetEndOfSelection()
        => Math.Max(_linesSelectionRange.Start, _linesSelectionRange.End) - CuiRender.TopOffset;

    protected override bool HandleInput()
    {
        _lastPressedKey = Settings.OutBuffer!.ReadKey().Key;

        if (_lastPressedKey == ConsoleKey.DownArrow)
        {
            if (_linesSelectionRange.End + 1 < TextBuffer.Size)
            {
                _linesSelectionRange.End++;  
                TextBuffer.IncCursorPositionFromTop();
                CuiRender.Render();
            }
        }
        else if (_lastPressedKey == ConsoleKey.UpArrow)
        {
            if (_linesSelectionRange.End > 0)
            {
                _linesSelectionRange.End--;
                TextBuffer.DecCursorPositionFromTop();
                CuiRender.Render();
                
            }
        }
        else if (_lastPressedKey is 
                 ConsoleKey.Enter or
                 ConsoleKey.Backspace or
                 ConsoleKey.Escape)
        {
            return true;
        }

        return false;
    }
    
    protected override void HandleSelectedOption()
    {
        if (_lastPressedKey == ConsoleKey.Enter)
            CopySelectedText();
        else if (_lastPressedKey == ConsoleKey.Backspace)
            RemoveSelectedText();
        else if (_lastPressedKey == ConsoleKey.Escape)
            return;
    }

    private void CopySelectedText()
    {
        int start = Math.Min(_linesSelectionRange.Start, _linesSelectionRange.End);
        int end = Math.Max(_linesSelectionRange.Start, _linesSelectionRange.End) + 1;
        string[] lines = TextBuffer.Lines;
        string[] selectedLines = lines[start..end];
        string text = string.Join("\n", selectedLines);
        
        new Clipboard().SetText(text);
    }

    private void RemoveSelectedText()
    {
        int start = Math.Min(_linesSelectionRange.Start, _linesSelectionRange.End) + 1;
        int end = Math.Max(_linesSelectionRange.Start, _linesSelectionRange.End) + 2;

        TextBuffer.RemoveTextBlock(new TextBlockPointer(start, end));
        _ = 0;
    }
}

internal struct ChangingRange
{
    public int Start { get; set; }
    public int End { get; set; }
}