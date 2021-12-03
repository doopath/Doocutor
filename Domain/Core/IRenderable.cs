using System.Collections.Generic;
using Domain.Core.Cursors;

namespace Domain.Core;

public interface IRenderable<T> : IEnumerable<string> where T : IEnumerable<string>
{
    T GetView();
    CursorPosition CursorPosition { get; }
    int Width { get; }
    int Height { get; }
}

