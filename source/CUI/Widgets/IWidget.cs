namespace CUI.Widgets;

public interface IWidget : IRenderable<List<string>>
{
    void OnSceneUpdated(List<string> scene);
    void OnMounted(Action unmount, Action action);
}
