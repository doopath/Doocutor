using System.Collections.Generic;

namespace Domain.Core.CodeBufferContents
{
    public interface ICodeBufferContent
    {
        List<string> SourceCode { get; }
        int CursorPositionFromTop { get; }
        int CursorPositionFromLeft { get; }
    }
}
