using System;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Core;

public class OutputColorizing
{
    public static void ColorizeForeground([NotNull] ConsoleColor color, [NotNull] Action action)
    {
        Console.ForegroundColor = color;
        action.Invoke();
        Console.ResetColor();
    }
}