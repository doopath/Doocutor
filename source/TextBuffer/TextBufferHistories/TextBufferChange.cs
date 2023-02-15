using Common;

namespace TextBuffer.TextBufferHistories;

/// <summary>
/// Presents a change of a code buffer.
/// The CodeBufferHistory object contains items like
/// this, that makes it possible to 'undo' and 'redo'
/// user's actions.
/// </summary>
public readonly struct TextBufferChange : ITextBufferChange
{
    /// <summary>
    /// A range that contains indexes of the NewChanges.
    /// </summary>
    public Range Range { get; init; }

    /// <summary>
    /// Old cursor state ( left; top ) coordinates.
    /// </summary>
    public CursorPosition OldCursorPosition { get; init; }

    /// <summary>
    /// New cursor state ( left; top ) coordinates.
    /// </summary>
    public CursorPosition NewCursorPosition { get; init; }


    /// <summary>
    /// Lines those replaced the ones at `Range`.
    /// </summary>
    public string[] NewChanges { get; init; }

    /// <summary>
    /// Lines those be replaced by the `NewChanges` at `Range`.
    /// </summary>
    public string[] OldState { get; init; }

    /// <summary>
    /// Type of the committed change.
    /// </summary>
    public TextBufferChangeType Type { get; init; } = TextBufferChangeType.UNNAMED;

    public TextBufferChange(Range range, CursorPosition oldCursorPosition, CursorPosition newCursorPosition, string[] newChanges, string[] oldState, TextBufferChangeType type)
    {
        Range = range;
        OldCursorPosition = oldCursorPosition;
        NewCursorPosition = newCursorPosition;
        NewChanges = newChanges;
        OldState = oldState;
        Type = type;
    }
}
