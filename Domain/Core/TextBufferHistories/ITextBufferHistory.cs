using System.Collections.Generic;

namespace Domain.Core.TextBufferHistories;

public interface ITextBufferHistory : IEnumerable<ITextBufferChange>
{
    uint Limit { get; set; }
    uint Size { get; }
    void Add(ITextBufferChange change);
    void Clear();
    bool IsEmpty();
    ITextBufferChange Undo();
    ITextBufferChange Redo();
}

