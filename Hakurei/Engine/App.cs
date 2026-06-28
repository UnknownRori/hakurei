using Hakurei.Engine.Math;
using Hakurei.Engine.Renderer;
using SDL3;

namespace Hakurei.Engine;

public class App
{
    internal protected Window _window;
    internal protected GPUDevice _device;
    private bool _running = true;

    GraphicsPipeline pipeline;
    GPURenderer renderer;
    GPUBuffer<Vertex> vertexBuffer;
    GPUBuffer<UInt32> indicesBuffer;
    Texture cat;

    private readonly Vertex[] _baseVertices = new Vertex[]
    {
        new() {Position = new Vec3(-0.5f, -0.5f, 0f), UV = new Vec2(0.0f, 1.0f) },
        new() {Position = new Vec3(0.5f, -0.5f, 0f),  UV = new Vec2(1.0f, 1.0f) },
        new() {Position = new Vec3(0f, 0.5f, 0f),     UV = new Vec2(0.5f, 0.0f) },
    };
    private float _angle = 0f;


    public App(string title, int width, int height, int flags)
    {
        if (!SDL.Init(SDL.InitFlags.Video)) return;

        _window = new Window("urmom", 800, 600, 0);
        _device = new GPUDevice(_window);
        ShaderCross.Init();

        cat = new Texture(_device, "Assets/cat.png");

        DefaultShader defaultShader = new DefaultShader(_device);
        pipeline = DefaultPipeline.CreatePipeline(_window, _device, defaultShader);
        renderer = new GPURenderer(_window, _device);
        vertexBuffer = new GPUBuffer<Vertex>(_device, SDL.GPUBufferUsageFlags.Vertex, 128);

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
        _angle += 0.02f;

        var rotated = new Vertex[_baseVertices.Length];
        float c = MathF.Cos(_angle);
        float s = MathF.Sin(_angle);

        for (int i = 0; i < _baseVertices.Length; i++)
        {
            var p = _baseVertices[i].Position;
            rotated[i] = new Vertex
            {
                Position = new Vec3(
                    p.x * c - p.y * s,
                    p.x * s + p.y * c,
                    p.z
                ),
                UV = _baseVertices[i].UV
            };
        }

        vertexBuffer.Upload(rotated, 0);
    }

    protected virtual void Draw()
    {
        renderer.AcquireSwapChain();
        renderer.ClearScreen(new Vec4(0.15f, 0.1f, 0.15f, 1f));
        renderer.BeginPass(pipeline);
            renderer.BindVertex(vertexBuffer);
            renderer.BindIndices(indicesBuffer);
            renderer.BindTexture(cat);
            renderer.DrawIndexed(3);
        renderer.EndPass();
        renderer.Commit();
    }
}
