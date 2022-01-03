using System;
using System.Collections.Generic;

namespace Domain.Core.Widgets;

public interface IWidget : IRenderable<List<string>>
{
    void OnSceneUpdated(List<string> scene);
    void OnMounted(Action unmount, Action action);
}
