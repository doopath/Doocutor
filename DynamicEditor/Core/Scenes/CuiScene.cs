using Domain.Core.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicEditor.Core.Scenes
{
    public class CuiScene : IScene
    {
        public List<string> CurrentScene { get; private set; }
        public event Action<List<string>> OnSceneUpdated;

        public void Compose(string code, int width, int height, int topOffset)
        {
            var bottomEdge = height - 1;
            var buffer = new string[bottomEdge];
            var output = PrepareOutput(code, width, height, topOffset);

            for (var i = 0; i < bottomEdge; i++)
                buffer[i] = output[i];

            CurrentScene = buffer.ToList();
            OnSceneUpdated?.Invoke(CurrentScene);
        }

        private List<string> PrepareOutput(string code, int width, int height, int topOffset)
        {
            var output = GetOutput(width, code, topOffset);

            if (output.Count < height)
            {
                var emptyLinesCount = height - output.Count;

                for (var i = 0; i < emptyLinesCount; i++)
                    output.Add(new string(' ', width));
            }

            return output;
        }

        private List<string> GetOutput(int width, string code, int topOffset)
            => code
                .Split("\n")[topOffset..]
                .Select(l => l + (l.Length < width ? new string(' ', width - l.Length) : string.Empty))
                .ToList();
    }
}
