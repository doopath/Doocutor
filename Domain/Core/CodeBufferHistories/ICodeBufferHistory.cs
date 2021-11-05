using System.Collections.Generic;

namespace Domain.Core.CodeBufferHistories
{
    public interface ICodeBufferHistory : IEnumerable<CodeBufferChange>
    {
        uint MaxLength { get; set; }
        uint Size { get; }
        void Add(CodeBufferChange change);
        void Clear();
        bool IsEmpty();
        CodeBufferChange Undo();
        CodeBufferChange Redo();
    }
}
