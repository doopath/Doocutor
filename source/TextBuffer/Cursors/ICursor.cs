namespace TextBuffer.Cursors
{
    public interface ICursor
    {
        int CursorPositionFromLeft { get; set; }
        int CursorPositionFromTop { get; set; }

        void SetCursorPositionFromTopAt(int position);
        void SetCursorPositionFromLeftAt(int position);
        public void IncCursorPositionFromLeft();
        public void DecCursorPositionFromLeft();
        public void IncCursorPositionFromTop();
        public void DecCursorPositionFromTop();
    }
}