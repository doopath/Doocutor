using Common;

namespace CUI.OutBuffers
{
    public interface IOutBuffer
    {
        int Width { get; set; }
        int Height { get; set; }
        bool CursorVisible { get; set; }
        int CursorTop { get; set; }
        int CursorLeft { get; set; }
        ConsoleKeyInfo ReadKey();
        void WriteLine(string line);
        void Write(string line);
        void Fill(IEnumerable<string> scene);
        void SetCursorPosition(int left, int top);
        void SetCursorPosition(CursorPosition position);
        void Clear();
    }
}