namespace Domain.Core.CodeBufferHistories
{
    public struct CodeBufferChange
    {
        public int LineIndex { get; init; }
        public string[] Changes { get; init; }

        public CodeBufferChange(int index, string[] changes)
        {
            LineIndex = index;
            Changes = changes;
        }
    }
}
