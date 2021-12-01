using System;
using System.Collections.Generic;
using System.Diagnostics;
using Domain.Core;
using Domain.Core.ColorSchemes;
using Domain.Core.OutBuffers;
using Domain.Core.Scenes;
using Domain.Core.TextBuffers;
using Pastel;

namespace DynamicEditor.Core;

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
    public static int TopOffset { get; set; }

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
        set
        {
            DeveloperMonitor.ColorScheme = value;
        }
    }

    private static readonly object RenderLocker = new();
    private static DeveloperMonitor? DeveloperMonitor;
    private static List<string> PureScene;
    private static int WindowWidth => OutBuffer!.Width;
    private static int WindowHeight => OutBuffer!.Height;
    private static int RightEdge => OutBuffer!.Width - 2;
    private static int BottomEdge => OutBuffer!.Height - 1;
    private static Stopwatch _watch;
    private static long _lastFrameRenderTime;
    private const int TopEdge = 0;

    /// <param name="codeBuffer">
    /// A code buffer (for example: Domain.Core.CodeBuffers.SourceCodeBuffer).
    /// </param>
    /// <param name="outBuffer">
    /// The out buffer which should be used to render a scene. For example:
    /// Domain.Core.OutBuffers.StandardConsoleOutBuffer.
    /// </param>
    /// <param name="scene">
    /// The scene which will be composed and rendered in the out buffer.
    /// </param>
    /// <param name="colorScheme">
    /// A color scheme which is used to colorize output.
    /// </param>
    static CuiRender()
    {
        _watch = new Stopwatch();
        PureScene = new();
        TopOffset = 0;
    }

    /// <summary>
    /// TextBuffer, Scene and ColorScheme must be set
    /// before the developer monitor initializing.
    /// </summary>
    public static void InitializeDeveloperMonitor()
        => DeveloperMonitor = new DeveloperMonitor(
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
        DeveloperMonitor!.TurnOn();
    }

    /// <summary>
    /// The DeveloperMonitor must be set before disabling it.
    /// </summary>
    public static void DisableDeveloperMonitor()
    {
        IsDeveloperMonitorShown = false;
        DeveloperMonitor!.TurnOff();
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
        lock (RenderLocker)
        {
            if (IsDeveloperMonitorShown)
                StartWatching();

            ShowScene(scene);
            RenderCursor();

            if (!IsDeveloperMonitorShown) return;

            StopWatching();
            UpdateDeveloperMonitor();
        }
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
    // It enhances performance, but is not comfy for debug.
    public void MoveCursorLeft()
        => DoHorizontalCursorMovement(_textBuffer.DecCursorPositionFromLeft);

    public void MoveCursorRight()
        => DoHorizontalCursorMovement(_textBuffer.IncCursorPositionFromLeft);
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
            RemoveCursor(initialLeftPosition, initialTopPosition);
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

    private static void RemoveCursor(int initialLeftPosition, int initialTopPosition)
    {
        char initialCursorSymbol = Scene!
            .CurrentScene![initialTopPosition][initialLeftPosition];

        OutBuffer!.SetCursorPosition(initialLeftPosition, initialTopPosition);
        OutBuffer.Write(initialCursorSymbol.ToString());
    }

    private static void RenderCursor()
    {
        var top = TextBuffer!.CursorPositionFromTop - TopOffset;
        var left = TextBuffer.CursorPositionFromLeft;
        var initialCursorPosition = (OutBuffer!.CursorLeft, OutBuffer.CursorTop);
        var symbol = PureScene[top][left];

        OutBuffer.SetCursorPosition(left, top);
        OutBuffer.Write(symbol
            .ToString()
            .Pastel(ColorScheme!.CursorForeground)
            .PastelBg(ColorScheme.CursorBackground));

        (OutBuffer.CursorLeft, OutBuffer.CursorTop) = initialCursorPosition;
    }

    private static void SetScene()
    {
        if (IsDeveloperMonitorShown)
            UpdateDeveloperMonitor();

        TextBuffer!.AdaptTextForBufferSize(RightEdge);

        Scene!.TargetWidth = WindowWidth;
        PureScene = Scene.GetNewScene(
            TextBuffer.CodeWithLineNumbers, WindowHeight, TopOffset);
        Scene.ComposeOf(PureScene);
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
            TopOffset++;
            Render();
        }
        else if (internalCursorPositionFromTop < TopEdge && isItNotFirstLine)
        {
            TopOffset--;
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


    private static void UpdateDeveloperMonitor()
        => DeveloperMonitor!.Update(
               TopOffset,
               TextBuffer!.CursorPositionFromTop,
               TextBuffer.CursorPositionFromLeft,
               (ulong)_lastFrameRenderTime);
}
