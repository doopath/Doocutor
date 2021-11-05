using System;
using Domain.Core.Exceptions;

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
        public Range Range { get; init; }
        
        /// <summary>
        /// Lines those replaced the ones at `Range`.
        /// </summary>
        public string[] NewChanges { get; init; }
        
        /// <summary>
        /// Lines those be replaced by the `NewChanges` at `Range`.
        /// </summary>
        public string[] OldState { get; init; }


        public CodeBufferChange(Range range, string[] newChanges, string[] oldState)
        {
            Range = new Range(range.Start, range.End);
            NewChanges = (string[]) newChanges.Clone();
            OldState = (string[]) oldState.Clone();
            
            Validate();
        }

        private void Validate()
        {
            int start = Range.Start.Value;
            int end = Range.End.Value;

            if (end - start != OldState.Length)
                throw new IncorrectRangeException("Given range is incorrect!"
                    + "The Difference between End and Start doesn't equal length of "
                    + "the oldState array!");
        }
    }
}
