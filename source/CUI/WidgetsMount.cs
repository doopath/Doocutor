using CUI.OutBuffers;
using CUI.Scenes;
using CUI.Widgets;

namespace CUI;

public static class WidgetsMount
{
    public static IOutBuffer OutBuffer { get; set; }
    public static IScene? Scene { get; set; }
    public static Action? Refresh { get; set; }

    static WidgetsMount()
    {
        OutBuffer = new StandardConsoleOutBuffer();
    }

    public static void Mount(IWidget widget)
    {
        var mounted = OnSceneUpdated(widget.OnSceneUpdated);
        Scene!.SceneUpdated += mounted;
        Refresh!.Invoke();
        widget.OnMounted(() => Unmount(mounted), Refresh!);
    }

    private static void Unmount(EventHandler<SceneUpdatedEventArgs> mounted)
    {
        Scene!.SceneUpdated -= mounted;
        Refresh!.Invoke();
    }
    private static EventHandler<SceneUpdatedEventArgs> OnSceneUpdated(Action<List<string>> action)
        => (object? sender, SceneUpdatedEventArgs eventArgs) => action(eventArgs.SceneContent!);
}
