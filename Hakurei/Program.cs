using Hakurei.Engine;

internal class Program
{
    static void Main(string[] args)
    {
        App game = new App("urmom", 800, 600, (int) SDL3.SDL.WindowFlags.AlwaysOnTop);
        game.Run();
    }
}
