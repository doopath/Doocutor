using System;
using System.Collections.Generic;
using Domain.Core.OutBuffers;
using Domain.Core.Scenes;
using Domain.Core.Widgets;

namespace Domain.Core;

public static class WidgetsMount
{
    public static IOutBuffer OutBuffer { get; set; }
    public static IScene? Scene { get; set; }
    public static Action? Refresh { get; set; }

    static WidgetsMount()
    {
        OutBuffer = new StandardConsoleOutBuffer();
    }

    public static void Mount(IWidget renderableObject)
    {
        var mounted = OnSceneUpdated(renderableObject.OnSceneUpdated);
        Scene!.SceneUpdated += mounted;
        Refresh!.Invoke();
        renderableObject.OnMounted(() => Unmount(mounted), Refresh!);
    }

    public static void Unmount(EventHandler<SceneUpdatedEventArgs> mounted)
    {
        Scene!.SceneUpdated -= mounted;
        Refresh!.Invoke();
    }
    private static EventHandler<SceneUpdatedEventArgs> OnSceneUpdated(Action<List<string>> action)
    => (object? sender, SceneUpdatedEventArgs eventArgs) => action(eventArgs.SceneContent!);
}
