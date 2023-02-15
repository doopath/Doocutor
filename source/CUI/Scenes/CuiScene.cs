using Utils.Exceptions.NotExitExceptions;

namespace CUI.Scenes
{
    public class CuiScene : IScene
    {
        public List<string>? CurrentScene { get; private set; }
        public event EventHandler<SceneUpdatedEventArgs>? SceneUpdated;
        public int? TargetWidth { get; set; }

        public void Compose(string code, int height, int topOffset)
        {
            CurrentScene = ComposeNewScene(code, height, topOffset);
            SceneUpdated?.Invoke(this, new() { SceneContent = CurrentScene });
        }

        public void ComposeOf(List<string> sceneContent)
        {
            CurrentScene = new List<string>(sceneContent);
            SceneUpdated?.Invoke(this, new() { SceneContent = CurrentScene });
        }

        public List<string> GetNewScene(string code, int height, int topOffset)
            => ComposeNewScene(code, height, topOffset);


        private List<string> ComposeNewScene(string code, int height, int topOffset)
        {
            RequireTargetWidth();

            var bottomEdge = height - 1;
            var buffer = new string[bottomEdge];
            var output = PrepareOutput(code, height, topOffset);

            for (var i = 0; i < bottomEdge; i++)
                buffer[i] = output[i];

            return buffer.ToList();
        }

        private List<string> PrepareOutput(string code, int height, int topOffset)
        {
            var output = GetOutput(code, topOffset);

            if (output.Count < height)
            {
                var emptyLinesCount = height - output.Count;

                for (var i = 0; i < emptyLinesCount; i++)
                    output.Add(new string(' ', TargetWidth!.Value));
            }

            return output;
        }

        private List<string> GetOutput(string code, int topOffset)
            => code
                .Split("\n")[topOffset..]
                .Select(l => l + (l.Length < TargetWidth
                    ? new string(' ', TargetWidth!.Value - l.Length)
                    : string.Empty))
                .ToList();

        private void RequireTargetWidth()
        {
            if (TargetWidth is null)
                throw new PropertyIsNotDefinedException("TargetWidth property isn't set!");
        }
    }
}
