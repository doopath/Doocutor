namespace Domain.Core.Cursors
{
    public readonly struct CursorPosition
    {
        public int Left { get; init; }
        public int Top { get; init; }

        public CursorPosition(int left, int top)
        {
            Left = left;
            Top = top;
        }
    }
}
