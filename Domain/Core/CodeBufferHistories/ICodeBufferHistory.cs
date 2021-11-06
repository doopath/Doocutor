using System.Collections.Generic;

namespace Domain.Core.CodeBufferHistories
{
    public interface ICodeBufferHistory : IEnumerable<ICodeBufferChange>
    {
        uint Limit { get; set; }
        uint Size { get; }
        void Add(ICodeBufferChange change);
        void Clear();
        bool IsEmpty();
        ICodeBufferChange Undo();
        ICodeBufferChange Redo();
    }
}
