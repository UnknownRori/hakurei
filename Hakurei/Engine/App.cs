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

    Camera2D camera;
    GPURenderer renderer;
    FullscreenRenderer fullscreenRenderer;
    Sprite2DBatcher catSpriteBatcher;
    GraphicsPipeline spriteBatcherPipeline;
    RenderTarget catRender;
    RenderTarget catRender2;
    Texture cat;
    private const float _cameraSpeed = 4f;
    private float rotation = 0f;

    public App(string title, int width, int height, int flags)
    {
        if (!SDL.Init(SDL.InitFlags.Video)) return;

        _window = new Window("urmom", 800, 600, 0);
        _device = new GPUDevice(_window);
        ShaderCross.Init();

        cat = new Texture(_device, "Assets/cat.png");

        renderer = new GPURenderer(_window, _device);
        fullscreenRenderer = new FullscreenRenderer(_window, renderer);
        spriteBatcherPipeline = GraphicPipeline2D.CreatePipeline(_window, _device);

        catSpriteBatcher = new Sprite2DBatcher(renderer);
        catRender = new RenderTarget(_device, 800, 600);
        catRender2 = new RenderTarget(_device, 400, 300);
        camera = new Camera2D();
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
        var states = SDL.GetKeyboardState(out _);
        rotation += 0.1f;

        if (states[(int)SDL.Scancode.W]) camera.Position.y += _cameraSpeed;
        if (states[(int)SDL.Scancode.S]) camera.Position.y -= _cameraSpeed;
        if (states[(int)SDL.Scancode.A]) camera.Position.x += _cameraSpeed;
        if (states[(int)SDL.Scancode.D]) camera.Position.x -= _cameraSpeed;

    }

    protected virtual void Draw()
    {
        renderer.AcquireSwapChain();

        UniformBlock uni = new UniformBlock(camera.GetViewProjection(renderer.SwapChainWidth, renderer.SwapChainHeight));

        renderer.SetRenderTarget(catRender);
        renderer.ClearScreen(new Vec4(0f, 0f, 0f, 1f));
        renderer.PushVertexUniform(uni);
        renderer.BeginPass(spriteBatcherPipeline);
        catSpriteBatcher.DrawSprite(cat, new Vec2(200.0f, 200.0f), new PackedColor(255, 255, 255, 255));
        catSpriteBatcher.DrawSpritePro(cat, new Vec2(100.0f, 100.0f), 2f, rotation, new PackedColor(255, 0, 0, 255));
        catSpriteBatcher.Flush();
        renderer.EndPass();

        fullscreenRenderer.Blit(catRender, catRender2);
        fullscreenRenderer.Blit(catRender2, null);
        renderer.EndPass();
        renderer.Commit();
    }
}
