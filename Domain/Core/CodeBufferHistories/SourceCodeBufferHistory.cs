using System.Collections.Generic;

namespace Domain.Core.CodeBufferHistories
{
    public class SourceCodeBufferHistory : ICodeBufferHistory
    {
        private List<CodeBufferChange> _history;
        private int _pointer;

        public SourceCodeBufferHistory()
        {
            _history = new();
            _pointer = 0;
        }

        public void Clear()
        {
            _history.Clear();
            _pointer = 0;
        }

        public bool IsEmpty()
            => _history.Count == 0;

        public void Add(CodeBufferChange change)
        {
            _history.Add(change);
            _pointer++;
        }

        public CodeBufferChange Redo()
        {
            _pointer++;
            return _history[_pointer-1];
        }

        public CodeBufferChange Undo()
        {
            _pointer--;
            return _history[_pointer+1];
        }
    }
}
