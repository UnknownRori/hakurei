namespace Hakurei.Engine;

public class Logger
{
    public static void Log(string name, string message)
    {
        Console.WriteLine($"[LOG] {name}: {message}");
    }

    public static void Fatal(string name, string message)
    {
        Console.WriteLine($"[FATAL] {name}: {message}");
        throw new Exception();
    }
}
