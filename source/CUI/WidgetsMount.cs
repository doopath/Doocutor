using CUI.OutBuffers;
using CUI.Scenes;
using CUI.Widgets;

namespace CUI;

public static class WidgetsMount
{
    public static IOutBuffer OutBuffer { get; set; }
    public static IScene? Scene { get; set; }

    static WidgetsMount()
    {
        OutBuffer = Settings.OutBuffer;
    }

    public static void Mount(IWidget widget)
    {
        var mounted = OnSceneUpdated(widget.OnSceneUpdated);
        Scene!.SceneUpdated += mounted;
        widget.OnMounted(() => Unmount(mounted));
    }

    private static void Unmount(EventHandler<SceneUpdatedEventArgs> mounted)
    {
        Scene!.SceneUpdated -= mounted;
        CuiRender.Render();
    }
    private static EventHandler<SceneUpdatedEventArgs> OnSceneUpdated(Action<List<string>> action)
        => (object? sender, SceneUpdatedEventArgs eventArgs) => action(eventArgs.SceneContent!);
}
