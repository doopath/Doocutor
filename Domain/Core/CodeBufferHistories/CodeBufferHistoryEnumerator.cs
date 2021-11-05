using System.Collections.Generic;

namespace Domain.Core.CodeBufferHistories
{
    internal class CodeBufferHistoryEnumerator : IEnumerator<CodeBufferChange>
    {
        CodeBufferChange IEnumerator<CodeBufferChange>.Current => (CodeBufferChange) Current;
        public object Current => _history[_pointer];
        
        private readonly CodeBufferChange[] _history;
        private readonly uint _initialPointer;
        private uint _pointer;

        public CodeBufferHistoryEnumerator(CodeBufferChange[] history, uint pointer)
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

        public void Dispose() {}
    }
}