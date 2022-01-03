using System.Collections;

namespace CUI.Widgets;

public class WidgetEnumerator : IEnumerator<string>
{
    private readonly IEnumerator<string> _linesEnum;
    public virtual string Current => _linesEnum.Current;
    object IEnumerator.Current => Current;

    public WidgetEnumerator(IEnumerable<string> lines)
    {
        _linesEnum = lines.GetEnumerator();
    }

    public bool MoveNext()
        => _linesEnum.MoveNext();

    public virtual void Reset()
        => _linesEnum.Reset();

    public virtual void Dispose()
        => _linesEnum.Dispose();
}
