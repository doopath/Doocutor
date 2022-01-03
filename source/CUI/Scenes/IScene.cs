namespace CUI.Scenes
{
    public interface IScene
    {
        int? TargetWidth { get; set; }
        event EventHandler<SceneUpdatedEventArgs> SceneUpdated;
        List<string>? CurrentScene { get; }
        List<string> GetNewScene(string code, int height, int topOffset);
        void ComposeOf(List<string> sceneContent);
        void Compose(string code, int height, int topOffset);
    }
}
