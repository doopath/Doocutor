using System;
using System.Collections;
using System.Collections.Generic;
using Domain.Core.Exceptions;

namespace Domain.Core.CodeBufferHistories
{
    /// <summary>
    /// A history of the source code buffer changes.
    /// It is used than a user wants to undo/redo changes.
    /// When you undo/redo changes they aren't removed, keep
    /// in mind it can takes a lot of memory (so just clear the history
    /// after some time).
    /// </summary>
    public class SourceCodeBufferHistory : ICodeBufferHistory
    {
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
        
        /// <summary>
        /// Max length of the history.
        /// Changes those older than MaxLength will
        /// be removed.
        /// </summary>
        public uint MaxLength { get; set; }
        public uint Size => (uint) _history.Count;
        
        private List<CodeBufferChange> _history;
        private int _pointer;

        /// <param name="maxLength">
        /// Max length of the history.
        /// Changes those older than MaxLength will
        /// be removed.
        /// </param>
        public SourceCodeBufferHistory(uint maxLength)
        {
            MaxLength = maxLength;
            _history = new();
            _pointer = -1;
        }

        public void Clear()
        {
            _history.Clear();
            _pointer = 0;
        }

        public bool IsEmpty()
            => _history.Count == -1;

        /// <summary>
        /// Add a change to the history.
        /// Also increases internal pointer to
        /// the added change.
        /// </summary>
        /// 
        /// <param name="change">
        /// A change of the source code buffer.
        /// </param>
        public void Add(CodeBufferChange change)
        {
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
        public CodeBufferChange Redo()
        {
            _pointer++;
            return _history[(int)_pointer-1];
        }

        /// <summary>
        /// Go to the previous change.
        /// </summary>
        /// 
        /// <returns>
        /// The previous change.
        /// </returns>
        public CodeBufferChange Undo()
        {
            _pointer--;
            return _history[(int)_pointer+1];
        }

        public IEnumerator<CodeBufferChange> GetEnumerator()
        {
            if (_pointer == -1)
                throw new ValueOutOfRangeException(
                    "Cannot get an enumerator of the empty history!");
                    
            return new CodeBufferHistoryEnumerator(_history.ToArray(), (uint) _pointer);
        }
            
    }
}
