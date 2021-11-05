using System;

namespace Domain.Core.CodeBufferHistories
{
    public interface ICodeBufferChange
    {
        public Range Range { get; init; }
        public string[] NewChanges { get; init; }
        public string[] OldState { get; init; }
    }
}
