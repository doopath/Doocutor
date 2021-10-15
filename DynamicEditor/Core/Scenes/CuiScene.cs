using Domain.Core.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicEditor.Core.Scenes
{
    internal class CuiScene : IScene
    {
        public List<string> CurrentScene { get; private set; }
        public event Action<List<string>> OnSceneUpdated;

        public void Compose(string code, int width, int height, int topOffset)
        {
            var buffer = new List<string>();
            var output = PrepareOutput(code, width, height, topOffset);
            var bottomEdge = height - 1;

            for (var i = 0; i < bottomEdge; i++)
                buffer.Add(output[i]);

            CurrentScene = buffer;
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
                .AsParallel()
                .Select(l => l + (l.Length < width ? new string(' ', width - l.Length) : string.Empty))
                .ToList();
    }
}
