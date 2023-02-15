using Pastel;
using System.Text.RegularExpressions;

namespace CUI.Widgets;

public class TextInputDialogWidget : DialogWidget
{
    protected string _inputFieldText;
    protected int _inputFieldOffset;
    protected int _inputFieldCursorIndex;
    protected virtual int InputFieldWidth => Width - _textRightEdge - _textLeftEdge;

    public TextInputDialogWidget(string text, Action? onCancel, Action<object?>? onOk) : base(text, onCancel, onOk)
    {
        _textLeftEdge = 2;
        _textRightEdge = 2;
        _textBottomEdge = 6;
        _textForegroundColor = ColorScheme!.TextForeground;
        _horizontalSymbol = "─".Pastel(ColorScheme.WidgetBorderForeground);
        _verticalSymbol = "│".Pastel(ColorScheme.WidgetBorderForeground);
        _topLeftCorner = "╭".Pastel(ColorScheme.WidgetBorderForeground);
        _topRightCorner = "╮".Pastel(ColorScheme.WidgetBorderForeground);
        _bottomLeftCorner = "╰".Pastel(ColorScheme.WidgetBorderForeground);
        _bottomRightCorner = "╯".Pastel(ColorScheme.WidgetBorderForeground);
        _activeButtonIndex = ButtonsMap.Keys.ToList().IndexOf(WidgetAction.OK.ToString());
        _inputFieldText = string.Empty;
        _inputFieldCursorIndex = _textLeftEdge;
        _inputFieldOffset = 0;

        OnCancel = onCancel;
        OnOk = onOk;

        UpdateWidth();
        UpdateHeight();

        Text = text;
        Items = new();

        Refresh();
    }

    protected override void Refresh()
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
        AddInputField();
    }

    protected virtual void AddInputField()
    {
        int topPosition = Height - _textBottomEdge;
        int cursorPosition = _inputFieldCursorIndex - _inputFieldOffset;

        string text = ComposeInputFieldText();
        AddInputFieldText(text, topPosition);
        AddInputFieldCursor(topPosition, cursorPosition);
    }

    protected virtual void AddInputFieldCursor(int topPosition, int index)
    {
        string cursorSymbol = RemoveAsciiColors(Items![topPosition][index]);
        Items![topPosition][index] = ColorizeInputFieldCursor(cursorSymbol);
    }

    protected virtual string ColorizeInputFieldCursor(string cursorSymbol)
        => cursorSymbol
            .Pastel(ColorScheme!.InputFieldCursorForeground)
            .PastelBg(ColorScheme.InputFieldCursorBackground);

    protected virtual void AddInputFieldText(string text, int topPosition)
    {
        for (int i = 0; i < text.Length; i++)
        {
            Items![topPosition][i + _textLeftEdge] = ColorizeInputFieldTextItem(text[i].ToString());
        }
    }

    protected virtual string ComposeInputFieldText()
    {
        string text = _inputFieldText;

        if (text.Length + 1 > InputFieldWidth)
            text = text[^(InputFieldWidth - 1)..];
        else if (text.Length < InputFieldWidth)
            text += new string(' ', InputFieldWidth - text.Length);

        return text;
    }

    protected virtual string ColorizeInputFieldTextItem(string item)
        => item.Pastel(ColorScheme!.InputFieldForeground)
            .PastelBg(ColorScheme.InputFieldBackground);

    protected override bool HandleInput()
    {
        ConsoleKeyInfo key = Settings.OutBuffer!.ReadKey();
        string inputTextPattern = @"[\/a-zA-Z0-9_.\s$%^&*-=\u005c|\[\]{}()\<\>;:'"",?!@#`~]+";
        string keyString = key.KeyChar.ToString();

        if (key.Key == ConsoleKey.RightArrow)
            _activeButtonIndex++;
        else if (key.Key == ConsoleKey.LeftArrow)
            _activeButtonIndex--;
        else if (key.Key == ConsoleKey.Enter)
            return true;
        else if (Regex.IsMatch(keyString, inputTextPattern))
            AppendSymbol(keyString);
        else if (key.Key == ConsoleKey.Backspace)
            Backspace();

        int buttonsCount = ButtonsMap.Keys.Count;
        _activeButtonIndex = ((_activeButtonIndex % buttonsCount)
                              + buttonsCount)
                              % buttonsCount;

        return false;
    }

    protected override void HandleSelectedOption()
    {
        string activeButton = ButtonsMap.Keys.ToArray()[_activeButtonIndex];

        if (activeButton == "OK")
            OnOk?.Invoke(_inputFieldText);
        else if (activeButton == "CANCEL")
            OnCancel?.Invoke();
    }

    protected virtual void AppendSymbol(string symbol)
    {
        _inputFieldText += symbol;
        _inputFieldCursorIndex++;

        if (_inputFieldCursorIndex - _textLeftEdge >= InputFieldWidth)
        {
            _inputFieldOffset++;
        }
    }

    protected virtual void Backspace()
    {
        if (_inputFieldText == string.Empty)
            return;

        _inputFieldText = _inputFieldText.Substring(0, _inputFieldText.Length - 1);
        _inputFieldCursorIndex--;

        if (_inputFieldOffset > 0)
            _inputFieldOffset--;
    }
}