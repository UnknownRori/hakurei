using Hakurei.Engine.Math;
using Hakurei.Engine.Renderer;
using Hakurei.Engine.Renderer2D;
using Hakurei.Engine.Audio;
using SDL3;
using Hakurei.Engine.TextRendering;

namespace Hakurei.Engine;

public class App : IDisposable
{
    protected bool _running = true;

    public App(string title, int width, int height, int flags)
    {
        HakureiEngine.Init(title, width, height, flags);
    }

    public void Dispose()
    {
        HakureiEngine.Dispose();
    }

    public virtual void Run()
    {
        while (_running)
        {
            while (SDL.PollEvent(out var ev))
            {
                PollEvent(ev);
            }
            Input.Update();

            Update();
            Draw();
        }
        Dispose();
    }


    protected virtual void PollEvent(SDL.Event ev)
    {
        switch ((SDL.EventType)ev.Type)
        {
            case SDL.EventType.Quit:
                _running = false;
                break;
        }
    }

    public virtual void Update()
    {

    }

    public virtual void Draw()
    {
        HakureiEngine.BeginDrawing();
            HakureiEngine.ClearScreen(new Vec4(0f, 0f, 0f, 1f));
            HakureiEngine.BeginPass(HakureiEngine.Pipeline2D);

            HakureiEngine.EndPass();
        HakureiEngine.EndDrawing();
    }
}
