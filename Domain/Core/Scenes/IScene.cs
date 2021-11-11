using System;
using System.Collections.Generic;

namespace Domain.Core.Scenes
{
    public interface IScene
    {
        int? TargetWidth { get; set; }
        event Action<List<string>> OnSceneUpdated;
        List<string>? CurrentScene { get; }
        List<string> GetNewScene(string code, int height, int topOffset);
        void ComposeOf(List<string> sceneContent);
        void Compose(string code, int height, int topOffset);
    }
}
