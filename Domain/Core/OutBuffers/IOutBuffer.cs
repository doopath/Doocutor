namespace Domain.Core.OutBuffers
{
    public interface IOutBuffer
    {
        int Width { get; set; }
        int Height { get; set; }
        bool CursorVisible { get; set; }
        void WriteLine(string line);
        void Write(string line);
        void SetCursorPosition(int left, int top);
        void Clear();
    }
}