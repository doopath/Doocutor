using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Common;
using CUI.ColorSchemes;
using CUI.OutBuffers;
using CUI.Scenes;
using Pastel;
using Utils.Exceptions.NotExitExceptions;

namespace CUI;

/// <summary>
/// Render some text to the out buffer.
/// Usually the out buffer is the stdin.
/// It doesn't display real cursor of the out buffer
/// (for being more faster), it uses the virtual one
/// (a colorized symbol).
/// </summary>
public static class CuiRender
{
    /// <summary>
    /// Offset value from the top of the code buffer.
    /// Is used to scrolling rendered content.
    /// </summary>
    public static int TopOffset { get; private set; }

    public static ITextBuffer? TextBuffer { get; set; }
    public static IScene? Scene { get; set; }
    public static IOutBuffer? OutBuffer { get; set; }

    /// <summary>
    /// Set color scheme for the rendered text and the
    /// developer monitor.
    /// </summary>
    public static IColorScheme ColorScheme
    {
        get => _colorScheme ?? Settings.ColorScheme;
        set => _colorScheme = value;
    }
    
    public const int TopEdge = 0;
    public static int WindowWidth => OutBuffer!.Width;
    public static int WindowHeight => OutBuffer!.Height;
    public static int RightEdge => OutBuffer!.Width - 2;
    public static int BottomEdge => OutBuffer!.Height - 1;
    public static long LastFrameRenderTime { get; private set; }

    private static IColorScheme? _colorScheme;
    private static List<string> _pureScene;
    private static Stopwatch _watch;
    private static bool _showCursor;

    static CuiRender()
    {
        _watch = new Stopwatch();
        _pureScene = new();
        _showCursor = true;
        TopOffset = 0;
    }

    /// <summary>
    /// Disables the cursor which is rendered (virtual).
    /// </summary>
    public static void DisableVirtualCursor()
        => _showCursor = false;

    /// <summary>
    /// Enables the cursor which is rendered (virtual).
    /// </summary>
    public static void EnableVirtualCursor()
        => _showCursor = true;

    public static void ScrollDown()
    {
        if (TopOffset > TextBuffer!.Size - BottomEdge)
            throw new ValueOutOfRangeException(
                "It is already the last line of the text buffer! You cannot scroll down!");

        TopOffset++;
    }

    public static void ScrollUp()
    {
        if (TopOffset <= 0)
            throw new ValueOutOfRangeException(
                "It is already the first line of the text buffer! You cannot scroll down!");
        TopOffset--;
    }

    /// <summary>
    /// Now it works not stable so it is not used.
    /// You can call this method if you want to increase the performance,
    /// but if you will asynchronously render too fast you'll probably get
    /// some errors like 'IndexOutOfRangeException' or something like that.
    /// </summary>
    public static async void RenderAsync()
        => await Task.Run(Render);

    public static void Render()
    {
        SetScene();
        Render(Scene!.CurrentScene!);
    }

    /// <summary>
    /// Render the scene.
    /// This method uses the monitor, so you can call
    /// it only in single-thread mode.
    /// If the IsDeveloperMonitorShown property equals True,
    /// than the developer monitor also will be rendered.
    /// </summary>
    /// 
    /// <param name="scene">
    /// A list of lines to be rendered.
    /// </param>
    public static void Render(IEnumerable<string> scene)
    {
        StartWatching();
        ShowScene(scene);
        if (_showCursor) RenderCursor();
        StopWatching();
    }

    public static void Clear()
        => OutBuffer!.Clear();

    /// <summary>
    /// Disable real (not virtual) cursor of the out buffer.
    /// </summary>
    public static void DisableOutBufferCursor()
        => OutBuffer!.CursorVisible = false;

    public static void MoveCursorUp()
        => DoVerticalCursorMovement(TextBuffer!.DecCursorPositionFromTop);

    public static void MoveCursorDown()
        => DoVerticalCursorMovement(TextBuffer!.IncCursorPositionFromTop);

#if DEBUG
    public static void MoveCursorLeft()
        => DoVerticalCursorMovement(TextBuffer!.DecCursorPositionFromLeft);

    public static void MoveCursorRight()
        => DoVerticalCursorMovement(TextBuffer!.IncCursorPositionFromLeft);
#else
    // These two methods don't update the screen.
    // It enhances performance, but it's not comfy for debugging.
    public static void MoveCursorLeft()
        => DoHorizontalCursorMovement(TextBuffer!.DecCursorPositionFromLeft);

    public static void MoveCursorRight()
        => DoHorizontalCursorMovement(TextBuffer!.IncCursorPositionFromLeft);
#endif

    private static void DoHorizontalCursorMovement(Action movement)
    {
        int initialLeftPosition = TextBuffer!.CursorPositionFromLeft;
        int initialTopPosition = TextBuffer.CursorPositionFromTop - TopOffset;

        try
        {
            movement?.Invoke();
        }
        finally
        {
            FixCursorPosition();
            RemoveVirtualCursor(initialLeftPosition, initialTopPosition);
            RenderCursor();
        }
    }

    private static void DoVerticalCursorMovement(Action movement)
    {
        try
        {
            movement?.Invoke();
        }
        finally
        {
            Render();
        }
    }

    private static void RenderCursor()
    {
        try
        {
            var top = TextBuffer!.CursorPositionFromTop - TopOffset;
            var left = TextBuffer.CursorPositionFromLeft;
            var symbol = _pureScene[top][left];

            OutBuffer!.SetCursorPosition(left, top);
            OutBuffer.Write(symbol
                .ToString()
                .Pastel(ColorScheme!.CursorForeground)
                .PastelBg(ColorScheme.CursorBackground));
        }
        catch (ArgumentOutOfRangeException) { }
    }

    [SuppressMessage("ReSharper.DPA", "DPA0003: Excessive memory allocations in LOH")]
    private static void SetScene()
    {
        TextBuffer!.AdaptTextForBufferSize(RightEdge);
        Scene!.TargetWidth = WindowWidth;

        _pureScene = Scene.GetNewScene(
            TextBuffer.CodeWithLineNumbers, WindowHeight, TopOffset);

        Scene.ComposeOf(_pureScene);
    }

    private static void ShowScene(IEnumerable<string> scene)
    {
        OutBuffer!.SetCursorPosition(0, 0);
        OutBuffer.WriteLine(string.Join("", scene));

        FixCursorPosition();
    }

    private static void StartWatching()
        => _watch.Start();

    private static void StopWatching()
    {
        _watch.Stop();
        LastFrameRenderTime = _watch.ElapsedMilliseconds;
        _watch = new Stopwatch();
    }

    private static void RemoveVirtualCursor(int initialLeftPosition, int initialTopPosition)
    {
        char initialCursorSymbol = Scene!
            .CurrentScene![initialTopPosition][initialLeftPosition];

        OutBuffer!.SetCursorPosition(initialLeftPosition, initialTopPosition);
        OutBuffer.Write(initialCursorSymbol.ToString());
    }

    private static void FixCursorPosition()
    {
        FixVerticalCursorPosition();
        FixHorizontalCursorPosition();
    }

    private static void FixVerticalCursorPosition()
    {
        var internalCursorPositionFromTop = TextBuffer!.CursorPositionFromTop - TopOffset;
        var isItNotFirstLine = TopOffset != 0;

        if (internalCursorPositionFromTop >= BottomEdge)
        {
            ScrollDown();
            Render();
        }
        else if (internalCursorPositionFromTop < TopEdge && isItNotFirstLine)
        {
            ScrollUp();
            Render();
        }
    }

    private static void FixHorizontalCursorPosition()
    {
        if (TextBuffer!.CursorPositionFromLeft > RightEdge)
        {
            if (TextBuffer.CursorPositionFromTop >= TextBuffer.Size)
                TextBuffer.IncreaseBufferSize();

            var targetPositionFromLeft = TextBuffer.GetPrefixLength() + 1;

            TextBuffer.SetCursorPositionFromLeftAt(targetPositionFromLeft);
            TextBuffer.IncCursorPositionFromTop();
            Render();
        }
    }
}
