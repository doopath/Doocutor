using System;
using System.Collections;
using System.Collections.Generic;
using Domain.Core.Exceptions;

namespace Domain.Core.CodeBufferHistories
{
    /// <summary />
    public class SourceCodeBufferHistory : ICodeBufferHistory
    {
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
        
        /// <summary>
        /// Limit of items in the history.
        /// Changes those older than MaxLength will
        /// be removed.
        /// </summary>
        public uint Limit
        {
            get => _limit;
            set
            {
                if (value == 0)
                    throw new ArgumentOutOfRangeException(
                        "Limit cannot be equal 0!");

                _limit = value;
            }
        }

        public uint Size => (uint) _history.Count;

        private List<ICodeBufferChange> _history;
        private int _pointer;
        private uint _limit;

        /// <param name="limit">
        /// Max length of the history.
        /// Changes those older than MaxLength will
        /// be removed.
        /// </param>
        public SourceCodeBufferHistory(uint limit)
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
        public void Add(ICodeBufferChange change)
        {
            if (_history.Count == Limit)
                _history.RemoveAt(0);

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
        public ICodeBufferChange Redo()
        {
            if (_pointer == _history.Count)
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
        public ICodeBufferChange Undo()
        {
            if (_pointer == -1)
                throw new ValueOutOfRangeException(
                    "You are already at the oldest change!");

            _pointer--;

            return _history[(int)_pointer+1];
        }

        public IEnumerator<ICodeBufferChange> GetEnumerator()
        {
            if (_pointer == -1)
                throw new ValueOutOfRangeException(
                    "Cannot get an enumerator of the empty history!");
                    
            return new CodeBufferHistoryEnumerator(_history.ToArray(), (uint) _pointer);
        }
            
    }
}
