namespace StudentResultSystem.UI;

/// <summary>Safe console input helpers (re-ask on invalid input).</summary>
public static class ConsoleHelper
{
    public static string ReadString(string prompt)
    {
        Console.Write(prompt);
        // null means the input stream ended (Ctrl+Z / piped input finished)
        return Console.ReadLine() ?? throw new EndOfStreamException();
    }

    public static int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
    {
        while (true)
        {
            string s = ReadString(prompt);
            if (int.TryParse(s, out int value) && value >= min && value <= max) return value;
            Console.WriteLine(max == int.MaxValue
                ? "  Please enter a valid number."
                : $"  Please enter a number between {min} and {max}.");
        }
    }

    public static double ReadDouble(string prompt, double min, double max)
    {
        while (true)
        {
            string s = ReadString(prompt);
            if (double.TryParse(s, out double value) && value >= min && value <= max) return value;
            Console.WriteLine($"  Please enter a number between {min} and {max}.");
        }
    }

    public static void Info(string message) => Print(message, ConsoleColor.Green);
    public static void Error(string message) => Print(message, ConsoleColor.Red);

    private static void Print(string message, ConsoleColor color)
    {
        var old = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = old;
    }
}
