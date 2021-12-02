using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Domain.Core.ColorSchemes;
using Domain.Core.Cursors;
using Libraries.Core;
using Pastel;

namespace Domain.Core.Widgets;

public abstract class Widget : IWidget
{
    #region Stuff declaring
    protected const float RelativeWidgetWidth = 0.5f;
    protected const float RelativeWidgetHeight = 0.5f;
    public event Action<WidgetAction>? KeyPressed;
    public static IColorScheme? ColorScheme { get; set; } = Settings.ColorScheme;
    public virtual CursorPosition CursorPosition { get; protected set; }

    [Range(4, int.MaxValue)]
    public virtual int Width { get; protected set; }

    [Range(5, int.MaxValue)]
    public virtual int Height { get; protected set; }

    public virtual string? Text { get; init; }
    protected virtual List<string> Lines => Items!.Select(sublist => string.Join("", sublist)).ToList();
    protected virtual List<List<string>>? Items { get; set; }
    protected int _activeButtonIndex;

    protected virtual Dictionary<string, WidgetAction> ButtonsMap { get; set; } = new(new[]
    {
        new KeyValuePair<string, WidgetAction>("OK", WidgetAction.OK),
    });

    protected string? _borderColor;
    protected string? _horizontalSymbol;
    protected string? _verticalSymbol;
    protected string? _topLeftCorner;
    protected string? _topRightCorner;
    protected string? _bottomLeftCorner;
    protected string? _bottomRightCorner;
    #endregion

    #region Public methods
    public virtual void OnSceneUpdated([NotNull] List<string> scene)
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

    public virtual void OnMounted([NotNull] Action unmount, [NotNull] Action render)
    {
        while (true)
        {
            if (ControlButtons())
            {
                HandleSelectedOption();
                break;
            }

            try
            {
                render.Invoke();
            }
            catch (ArgumentOutOfRangeException exc)
            {
                ErrorHandling.fileLogger.Error(exc);
            }
        }

        render.Invoke();
        unmount.Invoke();
    }

    public virtual IEnumerator<string> GetEnumerator()
        => new WidgetEnumerator(Lines);

    IEnumerator IEnumerable.GetEnumerator()
    => GetEnumerator();

    public virtual List<string> GetView()
        => Lines;
    #endregion

    #region Protected-Virtual methods
    protected virtual void Refresh()
    {
        CursorPosition = new()
        {
            Left = (Settings.OutBuffer!.Width - Width) / 2,
            Top = (Settings.OutBuffer!.Height - Height) / 2
        };

        Items!.Clear();
        UpdateWidth();
        UpdateHeight();
        FillItems();
        AddBorder();
        AddButtons();
        AddText();
    }

    protected virtual void UpdateWidth()
        => Width = (int)(Settings.OutBuffer!.Width * RelativeWidgetWidth);

    protected virtual void UpdateHeight()
        => Height = (int)(Settings.OutBuffer!.Height * RelativeWidgetHeight);

    protected virtual void FillItems()
    {
        for (int i = 0; i < Height; i++)
        {
            Items!.Add(new());

            for (int j = 0; j < Width; j++)
                Items[i].Add(" ");
        }
    }

    protected virtual void AddText()
    {
        string[] textItems = Text!
            .ToCharArray()
            .Select(c => c.ToString())
            .ToArray();
        int leftEdge = 2;
        int rightEdge = Width - 2;
        int bottomEdge = Height - 3;
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

    protected virtual void AddBorder()
    {
        for (int i = 0; i < Width; i++)
            (Items![0][i], Items[^1][i]) = (_horizontalSymbol!, _horizontalSymbol!);

        for (int i = 0; i < Height; i++)
            (Items![i][0], Items[i][^1]) = (_verticalSymbol!, _verticalSymbol!);

        (Items![0][0], Items[0][^1], Items[^1][0], Items[^1][^1]) =
            (_topLeftCorner!, _topRightCorner!, _bottomLeftCorner!, _bottomRightCorner!);
    }

    protected virtual void AddButtons()
    {
        int buttonsCount = ButtonsMap.Keys.Count;
        int buttonNumber = 1;
        int prevButtonWidth = 0;
        int bottomPos = 3;
        string[] buttons = ButtonsMap.Keys
            .Select(b => _verticalSymbol + b + _verticalSymbol)
            .ToArray();

        foreach (string button in ButtonsMap.Keys)
        {
            List<string> buttonItems = GetButtonItems(button);
            int leftPos = (Width - 2 - buttonsCount * 4)
                / buttonsCount
                / 2 * buttonNumber
                + prevButtonWidth;
            AddAditionalButtonSymbols(buttonItems);

            for (int i = leftPos; i < leftPos + buttonItems.Count; i++)
            {
                string content = IsActiveButton(button)
                    ? ToInactiveButton(buttonItems[i - leftPos])
                    : ToActiveButton(buttonItems[i - leftPos]);
                Items![^bottomPos][leftPos - 2] = (IsActiveButton(button) ? ">" : " ")
                    .Pastel(ColorScheme!.TextForeground);
                Items[^bottomPos][i] = content;
            }

            buttonNumber++;
            prevButtonWidth += buttonItems.Count;
        }
    }

    protected virtual bool ControlButtons()
    {
        ConsoleKeyInfo key = Settings.OutBuffer!.ReadKey();

        if (key.Key == ConsoleKey.RightArrow)
            _activeButtonIndex += 1;
        else if (key.Key == ConsoleKey.LeftArrow)
            _activeButtonIndex -= 1;
        else if (key.Key == ConsoleKey.Enter)
            return true;

        _activeButtonIndex %= ButtonsMap.Keys.Count;

        return false;
    }

    protected virtual void HandleSelectedOption()
    {
        string activeButton = ButtonsMap.Keys.ToArray()[_activeButtonIndex];

        KeyPressed?.Invoke(ButtonsMap[activeButton]);
    }

    protected virtual List<string> GetButtonItems(string button)
        => button.ToCharArray().Select(i => i.ToString()).ToList();

    protected virtual void AddAditionalButtonSymbols(List<string> buttonItems)
    {
        buttonItems.Insert(0, _verticalSymbol!);
        buttonItems.Insert(buttonItems.Count, _verticalSymbol!);
        buttonItems.Insert(1, " ");
        buttonItems.Insert(buttonItems.Count - 1, " ");
    }

    protected virtual bool IsActiveButton(string buttonName)
        => buttonName == ButtonsMap.Keys.ToArray()[_activeButtonIndex];

    protected virtual string ToActiveButton(string button)
        => button.PastelBg(ColorScheme!.TextBackground)
                 .Pastel(ColorScheme!.TextForeground);

    protected virtual string ToInactiveButton(string button)
        => button.PastelBg(ColorScheme!.TextForeground)
                 .Pastel(ColorScheme!.TextBackground);
    #endregion
}

public enum WidgetAction
{
    OK,
    CANCEL,
    NO,

}