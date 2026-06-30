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
    Sprite2DBatcher catSpriteBatcher;
    RenderTarget catRender;

    private GPUBuffer<Vertex> vertexBuffer;
    private GPUBuffer<UInt32> indicesBuffer;
    private GraphicsPipeline pipeline;

    Texture cat;
    private const float _cameraSpeed = 4f;

    public App(string title, int width, int height, int flags)
    {
        if (!SDL.Init(SDL.InitFlags.Video)) return;

        _window = new Window("urmom", 800, 600, 0);
        _device = new GPUDevice(_window);
        ShaderCross.Init();

        cat = new Texture(_device, "Assets/cat.png");

        renderer = new GPURenderer(_window, _device);

        vertexBuffer = new GPUBuffer<Vertex>(renderer.Device, SDL.GPUBufferUsageFlags.Vertex, 4);
        indicesBuffer = new GPUBuffer<uint>(renderer.Device, SDL.GPUBufferUsageFlags.Index, 6);
        pipeline = DefaultPipeline.CreatePipeline(_window, _device, new DefaultShader(_device));

        catSpriteBatcher = new Sprite2DBatcher(renderer, cat);
        catRender = new RenderTarget(_device, 800, 600);
        catSpriteBatcher.SetRenderTarget(catRender);
        camera = new Camera2D();
        var vertices = new Vertex[]
        {
            new () { Position = new Vec3(-1f, -1f, 0f), UV = new Vec2(0f, 1f) },
            new () { Position = new Vec3(1f , -1f, 0f), UV = new Vec2(1f, 1f) },
            new () { Position = new Vec3(1f , 1f , 0f), UV = new Vec2(1f, 0f) },
            new () { Position = new Vec3(-1f, 1f , 0f), UV = new Vec2(0f, 0f) },
        };

        var indexes = new UInt32[]
        {
            0, 1, 2,
            2, 3, 0,
        };

        vertexBuffer.Upload(vertices);
        indicesBuffer.Upload(indexes);

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

        if (states[(int)SDL.Scancode.W]) camera.Position.y += _cameraSpeed;
        if (states[(int)SDL.Scancode.S]) camera.Position.y -= _cameraSpeed;
        if (states[(int)SDL.Scancode.A]) camera.Position.x += _cameraSpeed;
        if (states[(int)SDL.Scancode.D]) camera.Position.x -= _cameraSpeed;

        catSpriteBatcher.BindCamera(camera);
        catSpriteBatcher.PushSprite(new Vec2(200.0f, 200.0f), new PackedColor(255, 255, 255, 255));
        catSpriteBatcher.PushSprite(new Vec2(100.0f, 100.0f), new PackedColor(255, 0, 0, 255));
    }

    protected virtual void Draw()
    {
        renderer.AcquireSwapChain();
            catSpriteBatcher.Submit();
            
            renderer.SetRenderTarget(null);
            renderer.ClearScreen(new Vec4(0f, 1f, 0f, 1f));
            renderer.BeginPass(pipeline);
                renderer.BindVertex(vertexBuffer);
                renderer.BindIndices(indicesBuffer);
                renderer.BindTexture(catRender.Texture);
                renderer.DrawIndexed(6);
            renderer.EndPass();
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
