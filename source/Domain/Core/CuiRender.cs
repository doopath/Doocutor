using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Domain.Core.ColorSchemes;
using Domain.Core.Exceptions.NotExitExceptions;
using Domain.Core.OutBuffers;
using Domain.Core.Scenes;
using Domain.Core.TextBuffers;
using Pastel;

namespace Domain.Core;

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
    /// Enable/disable developer monitor
    /// (a panel that's displayed at the right top corner
    /// of the out buffer and shows render time, position, etc).
    /// Usually disabled in 'release' version.
    /// </summary>
    public static bool IsDeveloperMonitorShown { get; set; }

    /// <summary>
    /// Set color scheme for the rendered text and the
    /// developer monitor.
    /// </summary>
    public static IColorScheme? ColorScheme
    {
        get => Settings.ColorScheme;
        set => DeveloperMonitor.ColorScheme = value!;
    }

    private static DeveloperMonitor? _developerMonitor;
    private static List<string> _pureScene;
    private static int WindowWidth => OutBuffer!.Width;
    private static int WindowHeight => OutBuffer!.Height;
    private static int RightEdge => OutBuffer!.Width - 2;
    private static int BottomEdge => OutBuffer!.Height - 1;
    private static Stopwatch _watch;
    private static long _lastFrameRenderTime;
    private static bool _showCursor;
    private const int TopEdge = 0;

    static CuiRender()
    {
        _watch = new Stopwatch();
        _pureScene = new();
        _showCursor = true;
        TopOffset = 0;
    }

    /// <summary>
    /// TextBuffer, Scene and ColorScheme must be set
    /// before the developer monitor initializing.
    /// </summary>
    public static void InitializeDeveloperMonitor()
        => _developerMonitor = new DeveloperMonitor(
            TopOffset,
            TextBuffer!.CursorPositionFromTop,
            TextBuffer.CursorPositionFromLeft,
            Scene!,
            ColorScheme!);

    /// <summary>
    /// The DeveloperMonitor must be set before enabling it.
    /// </summary>
    public static void EnableDeveloperMonitor()
    {
        IsDeveloperMonitorShown = true;
        _developerMonitor!.TurnOn();
    }

    /// <summary>
    /// The DeveloperMonitor must be set before disabling it.
    /// </summary>
    public static void DisableDeveloperMonitor()
    {
        IsDeveloperMonitorShown = false;
        _developerMonitor!.TurnOff();
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
        if (IsDeveloperMonitorShown)
            StartWatching();

        ShowScene(scene);
        if (_showCursor) RenderCursor();

        if (!IsDeveloperMonitorShown) return;

        StopWatching();
        UpdateDeveloperMonitorAsync();
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
    // It enhances performance, but is not comfy for debugging.
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
        if (IsDeveloperMonitorShown)
            UpdateDeveloperMonitorAsync();

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
        _lastFrameRenderTime = _watch.ElapsedMilliseconds;
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

    private static async void UpdateDeveloperMonitorAsync()
        => await Task.Run(UpdateDeveloperMonitor);

    private static void UpdateDeveloperMonitor()
        => _developerMonitor!.Update(
               TopOffset,
               TextBuffer!.CursorPositionFromTop,
               TextBuffer.CursorPositionFromLeft,
               (ulong)_lastFrameRenderTime);
}
