namespace CUI.Scenes;

public class SceneUpdatedEventArgs : EventArgs
{
    public List<string>? SceneContent { get; init; }
}
