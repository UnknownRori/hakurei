using SDL3;
using Hakurei.Engine;

namespace Hakurei.Engine.Renderer;
public class Window: IDisposable
{
    private nint _window;
    public nint window { get => _window; }

    public Window(nint window) { _window = window;  }

    public Window(string title, int width, int height, int flags)
    {
        _window = SDL.CreateWindow(title, width, height, (SDL.WindowFlags)flags);
        Logger.Log("Renderer", $"SDL Window initialized ({width}:{height})");
    }

    public void Dispose()
    {
        if (_window == nint.Zero) return;
        SDL.DestroyWindow(_window);
        _window = nint.Zero;
    }
}
