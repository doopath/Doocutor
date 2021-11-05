using System.Collections.Generic;

namespace Domain.Core.CodeBufferHistories
{
    internal class CodeBufferHistoryEnumerator : IEnumerator<ICodeBufferChange>
    {
        ICodeBufferChange IEnumerator<ICodeBufferChange>.Current => (ICodeBufferChange) Current;
        public object Current => _history[_pointer];
        
        private readonly ICodeBufferChange[] _history;
        private readonly uint _initialPointer;
        private uint _pointer;

        public CodeBufferHistoryEnumerator(ICodeBufferChange[] history, uint pointer)
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
