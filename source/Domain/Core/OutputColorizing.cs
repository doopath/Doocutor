using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Domain.Core;

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