namespace Domain.Core.CodeBufferHistories
{
    /// <summary>
    /// Presents a change of a code buffer.
    /// The CodeBufferHistory object contains items like
    /// this, that makes it possible to 'undo' and 'redo'
    /// user's actions.
    /// </summary>
    public struct CodeBufferChange
    {
        /// <summary>
        /// An index of a line that was changed.
        /// </summary>
        public int LineIndex { get; init; }
        
        /// <summary>
        /// A line or lines those replaced the one at `index`.
        /// </summary>
        public string[] Changes { get; init; }
    }
}
