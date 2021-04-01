using Doocutor.Core.Exceptions;

namespace Doocutor.Core.CodeBuffers.CodePointers
{
    class CodeBlockPointer : ICodeBlockPointer
    {
        public int StartLineNumber { get; init; }
        public int EndLineNumber { get; init; }

        public CodeBlockPointer(int start, int end)
        {
            ValidateStartAndEndFields(start, end);
            this.StartLineNumber = start;
            this.EndLineNumber = end;
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
}