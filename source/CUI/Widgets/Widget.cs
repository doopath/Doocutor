using System.Collections;
using System.ComponentModel.DataAnnotations;
using Common;
using CUI.ColorSchemes;
using Pastel;

namespace CUI.Widgets;

public abstract class Widget : IWidget
{
    #region Stuff declaring
    #region Public fields, properties and events
    public static IColorScheme? ColorScheme { get; set; } = Settings.ColorScheme;

    [Range(4, int.MaxValue)]
    public virtual int Width { get; protected set; }

    [Range(5, int.MaxValue)]
    public virtual int Height { get; protected set; }

    public virtual string? Text { get; set; }
    public virtual event Action<WidgetAction>? KeyPressed;
    public virtual CursorPosition CursorPosition { get; protected set; }
    #endregion

    #region Protected fields and properties
    protected virtual float RelativeWidgetWidth { get; set; } = 0.5f;
    protected virtual float RelativeWidgetHeight { get; set; } = 0.5f;
    protected virtual List<string> Lines => Items!
        .Select(sublist => string.Join("", sublist))
        .ToList();
    protected virtual List<List<string>>? Items { get; set; }
    protected int _activeButtonIndex;

    protected virtual Dictionary<string, WidgetAction> ButtonsMap { get; set; } = new(new[]
    {
        new KeyValuePair<string, WidgetAction>("OK", WidgetAction.OK),
    });

    protected int? _textRightEdge;
    protected int? _textLeftEdge;
    protected int? _textBottomEdge;

    protected string? _textForegroundColor;
    protected string? _horizontalSymbol;
    protected string? _verticalSymbol;
    protected string? _topLeftCorner;
    protected string? _topRightCorner;
    protected string? _bottomLeftCorner;
    protected string? _bottomRightCorner;
    #endregion
    #endregion

    #region Public methods
    public virtual void OnSceneUpdated(List<string> scene)
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

    public virtual void OnMounted(Action unmount, Action render)
    {
        CuiRender.DisableVirtualCursor();
        render();
        
        while (true)
        {
            if (HandleInput())
            {
                HandleSelectedOption();
                break;
            }

            try
            {
                render();
            }
            catch (ArgumentOutOfRangeException exc)
            {
                ErrorHandling.FileLogger.Error(exc);
            }
        }

        CuiRender.EnableVirtualCursor();
        unmount();
        render();
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
        FillByEmptyItems();
        AddBorder();
        AddButtons();
        AddText();
    }

    protected virtual void UpdateWidth()
        => Width = (int)(Settings.OutBuffer!.Width * RelativeWidgetWidth);

    protected virtual void UpdateHeight()
        => Height = (int)(Settings.OutBuffer!.Height * RelativeWidgetHeight);

    protected virtual void FillByEmptyItems()
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
            .Select(c => c.ToString().Pastel(_textForegroundColor))
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
        int bottomPos = 3;
        string[] buttons = ButtonsMap.Keys
            .Select(b => _verticalSymbol + b + _verticalSymbol)
            .ToArray();

        foreach (string button in ButtonsMap.Keys)
        {
            List<string> buttonItems = GetButtonItems(button);
            int contentWidth = Width - 2;
            int chunkWidth = contentWidth / buttonsCount - 1;
            int leftPos = chunkWidth * (buttonNumber - 1) + chunkWidth / 2;

            AddAdditionalButtonSymbols(buttonItems);

            for (int i = leftPos; i < leftPos + buttonItems.Count; i++)
            {
                int buttonItemsIndex = i - leftPos;
                string content = IsActiveButton(button)
                    ? ToActiveButton(buttonItems[buttonItemsIndex])
                    : ToInactiveButton(buttonItems[buttonItemsIndex]);
                string selectedButtonSymbol = (IsActiveButton(button) ? ">" : " ")
                    .Pastel(ColorScheme!.TextForeground);

                Items![^bottomPos][leftPos - 2] = selectedButtonSymbol;
                Items[^bottomPos][i] = content;
            }

            buttonNumber++;
        }
    }

    protected virtual bool HandleInput()
    {
        ConsoleKeyInfo key = Settings.OutBuffer!.ReadKey();

        if (key.Key == ConsoleKey.RightArrow)
            _activeButtonIndex++;
        else if (key.Key == ConsoleKey.LeftArrow)
            _activeButtonIndex--;
        else if (key.Key == ConsoleKey.Enter)
            return true;

        int buttonsCount = ButtonsMap.Keys.Count;
        _activeButtonIndex = ((_activeButtonIndex % buttonsCount)
            + buttonsCount)
            % buttonsCount;

        return false;
    }

    protected virtual void HandleSelectedOption()
    {
        string activeButton = ButtonsMap.Keys.ToArray()[_activeButtonIndex];

        KeyPressed?.Invoke(ButtonsMap[activeButton]);
    }

    protected virtual List<string> GetButtonItems(string button)
        => button.ToCharArray().Select(i => i.ToString()).ToList();

    protected virtual void AddAdditionalButtonSymbols(List<string> buttonItems)
    {
        buttonItems.Insert(0, "[");
        buttonItems.Insert(buttonItems.Count, "]");
        buttonItems.Insert(1, " ");
        buttonItems.Insert(buttonItems.Count - 1, " ");
    }

    protected virtual bool IsActiveButton(string buttonName)
        => buttonName == ButtonsMap.Keys.ToArray()[_activeButtonIndex];

    protected virtual string ToActiveButton(string button)
        => button.PastelBg(ColorScheme!.WidgetActiveButtonBackground)
                 .Pastel(ColorScheme!.WidgetActiveButtonForeground);

    protected virtual string ToInactiveButton(string button)
        => button.PastelBg(ColorScheme!.WidgetInactiveButtonBackground)
                 .Pastel(ColorScheme!.WidgetInactiveButtonForeground);
    #endregion
}

public enum WidgetAction
{
    OK,
    CANCEL,
    NO,

}