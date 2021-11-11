using Domain.Core.Exceptions;

namespace Domain.Core.TextBuffers.TextPointers;


public class TextBlockPointer : ITextBlockPointer
{
    public int StartLineNumber { get; init; }
    public int EndLineNumber { get; init; }

    public TextBlockPointer(int start, int end)
    {
        ValidateStartAndEndFields(start, end);
        StartLineNumber = start;
        EndLineNumber = end;
    }

    private void ValidateStartAndEndFields(int start, int end)
    {
        if (start > end)
        {
            throw new CodeBlockPointerValidationException("End line number cannot be greater than start one!");
        }

        if (start == 0 || end == 0)
        {
            throw new CodeBlockPointerValidationException(
                "You cannot set 0 as end or start line number! Lowest line number is first.");
        }
    }
}
