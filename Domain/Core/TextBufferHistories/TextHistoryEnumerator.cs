using System.Collections.Generic;

namespace Domain.Core.TextBufferHistories;

internal class TextBufferHistoryEnumerator : IEnumerator<ITextBufferChange>
{
    ITextBufferChange IEnumerator<ITextBufferChange>.Current => (ITextBufferChange)Current;
    public object Current => _history[_pointer];

    private readonly ITextBufferChange[] _history;
    private readonly uint _initialPointer;
    private uint _pointer;

    public TextBufferHistoryEnumerator(ITextBufferChange[] history, uint pointer)
    {
        _history = history;
        _pointer = pointer;
        _initialPointer = pointer;
    }

    public bool MoveNext()
    {
        if (_pointer > 0)
            _pointer--;
        else
            return false;

        return true;
    }

    public void Reset()
        => _pointer = _initialPointer;

    public void Dispose() { }
}
