using Hakurei.Engine.Math;
using Hakurei.Engine.Renderer;
using Hakurei.Engine.Renderer2D;
using SDL3;

namespace Hakurei.Engine;

public class App
{
    internal protected Window _window;
    internal protected GPUDevice _device;
    private bool _running = true;

    GPURenderer renderer;
    Sprite2DBatcher catSpriteBatcher;

    Texture cat;


    public App(string title, int width, int height, int flags)
    {
        if (!SDL.Init(SDL.InitFlags.Video)) return;

        _window = new Window("urmom", 800, 600, 0);
        _device = new GPUDevice(_window);
        ShaderCross.Init();

        cat = new Texture(_device, "Assets/cat.png");

        renderer = new GPURenderer(_window, _device);
        catSpriteBatcher = new Sprite2DBatcher(renderer, cat);
    }

    ~App()
    {
        _device.Dispose();
        _window.Dispose();
        ShaderCross.Quit();
        SDL.Quit();
    }

    public virtual void Run()
    {
        while (_running)
        {
            while (SDL.PollEvent(out var ev))
            {
                PollEvent(ev);
            }

            Update();
            Draw();
        }
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

    protected virtual void Update()
    {
        catSpriteBatcher.PushSprite(new Vec2(200.0f, 200.0f), new PackedColor(255, 255, 255, 255));
        catSpriteBatcher.PushSprite(new Vec2(100.0f, 100.0f), new PackedColor(255, 0, 0, 255));
    }

    protected virtual void Draw()
    {
        renderer.AcquireSwapChain();
            catSpriteBatcher.Submit();
        renderer.Commit();
        
        //renderer.ClearScreen(new Vec4(0.15f, 0.1f, 0.15f, 1f));
        //renderer.BeginPass(pipeline);
        //    renderer.PushVertexUniform(new Vec4(1.0f, 0.0f, 0.0f, 1.0f));
        //    renderer.BindVertex(vertexBuffer);
        //    renderer.BindIndices(indicesBuffer);
        //    renderer.BindTexture(cat);
        //    renderer.DrawIndexed(6);
        //renderer.EndPass();
        //renderer.Commit();
    }
}
