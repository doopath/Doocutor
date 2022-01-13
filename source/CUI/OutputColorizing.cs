using System.Diagnostics.CodeAnalysis;

namespace CUI;

public class OutputColorizing
{
    public static void ColorizeForeground([NotNull] ConsoleColor color, [NotNull] Action action)
    {
        try
        {
            Console.ForegroundColor = color;
            action.Invoke();
            Console.ResetColor();
        }
        catch (IOException)
        {
            return;
        }
    }
}