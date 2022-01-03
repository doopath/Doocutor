using Common;
using TextBuffer.Cursors;

namespace TextBuffer.TextBufferHistories
{
    public interface ITextBufferChange
    {
        public Range Range { get; init; }
        public CursorPosition OldCursorPosition { get; init; }
        public CursorPosition NewCursorPosition { get; init; }
        public string[] NewChanges { get; init; }
        public string[] OldState { get; init; }
        public TextBufferChangeType Type { get; init; }
    }
}
