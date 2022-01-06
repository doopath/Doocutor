using System.Collections;
using Utils.Exceptions.NotExitExceptions;

namespace TextBuffer.TextBufferHistories;

/// <summary>
/// History of the SourceCodeBuffer.
/// You can add redo and undo added changes.
/// <summary />
public class TextBufferHistory : ITextBufferHistory
{
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <summary>
    /// Limit of items in the history.
    /// Changes those older than MaxLength will
    /// be removed.
    /// </summary>
    public int Limit
    {
        get => _limit;
        set
        {
            if (value == 0)
                throw new ArgumentOutOfRangeException("Limit cannot be equal 0!");

            _limit = value;
        }
    }

    public uint Size => (uint)_history.Count;

    private readonly List<ITextBufferChange> _history;
    private int _pointer;
    private int _limit;

    /// <param name="limit">
    /// Max length of the history.
    /// Changes those older than MaxLength will
    /// be removed.
    /// </param>
    public TextBufferHistory(int limit)
    {
        Limit = limit;
        _history = new();
        _pointer = -1;
    }

    public void Clear()
    {
        _history.Clear();
        _pointer = -1;
    }

    public bool IsEmpty()
        => _history.Count == 0;

    /// <summary>
    /// Add a change to the history.
    /// Also increases internal pointer to
    /// the added change.
    /// </summary>
    /// 
    /// <param name="change">
    /// A change of the source code buffer.
    /// </param>
    public void Add(ITextBufferChange change)
    {
        if (_history.Count == Limit)
            RemoveOldest();

        if (IsNotFirstLine())
            RemoveNewer();

        _history.Add(change);
        _pointer++;
    }

    /// <summary>
    /// Go to the next change.
    /// </summary>
    /// 
    /// <returns>
    /// The next change.
    /// </returns>
    public ITextBufferChange Redo()
    {
        if (_pointer == _history.Count - 1)
            throw new ValueOutOfRangeException(
                "You are already at the latest change!");

        _pointer++;

        return _history[_pointer];
    }

    /// <summary>
    /// Go to the previous change.
    /// </summary>
    /// 
    /// <returns>
    /// The previous change.
    /// </returns>
    public ITextBufferChange Undo()
    {
        if (_pointer == -1)
            throw new ValueOutOfRangeException(
                "You are already at the oldest change!");

        _pointer--;

        return _history[_pointer + 1];
    }

    public IEnumerator<ITextBufferChange> GetEnumerator()
    {
        if (_pointer == -1)
            throw new ValueOutOfRangeException(
                "Cannot get an enumerator of the empty history!");

        return new TextBufferHistoryEnumerator(_history.ToArray(), (uint)_pointer);
    }

    private void RemoveOldest()
    {
        _history.RemoveAt(0);
        _pointer -= _pointer == -1 ? 0 : 1;
    }

    private bool IsNotFirstLine()
        => _history.Count - 1 != _pointer;

    private void RemoveNewer()
    {
        int start = _pointer > 0 ? _pointer - 1 : 0;
        int count = _history.Count - _pointer - 1;

        _history.RemoveRange(start, count);
        _pointer = _history.Count - 1;
    }
}
