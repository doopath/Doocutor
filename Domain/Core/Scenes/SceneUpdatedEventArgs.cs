using System;
using System.Collections.Generic;

namespace Domain.Core.Scenes;

public class SceneUpdatedEventArgs : EventArgs
{
    public List<string>? SceneContent { get; init; }
}
