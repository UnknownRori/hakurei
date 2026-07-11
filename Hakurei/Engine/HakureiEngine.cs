using Hakurei.Engine.Audio;
using Hakurei.Engine.Math;
using Hakurei.Engine.Renderer;
using Hakurei.Engine.Renderer2D;
using Hakurei.Engine.TextRendering;
using SDL3;

namespace Hakurei.Engine;

public class HakureiEngine
{
    static protected Window window;
    static protected GPUDevice device;
    static protected GPURenderer renderer;
    static protected Sprite2DBatcher sprite2DBatcher;
    static protected GraphicsPipeline pipeline2D;
    static protected FullscreenRenderer fullscreenRenderer;
    static protected AudioMixer mixer;
    static protected GPUTextRendering textRenderer;

    static public Window Window { get { return window; } }
    static public GPUDevice Device { get { return device; } }
    static public GPURenderer Renderer { get { return renderer; } }
    static public GraphicsPipeline Pipeline2D { get { return pipeline2D; } }
    static public Sprite2DBatcher Sprite2DBatcher { get { return sprite2DBatcher; } }
    static public GPUTextRendering TextRenderer { get { return textRenderer; } }

    public static void Init(string title, int width, int height, int flags)
    {
        if (!SDL.Init(SDL.InitFlags.Video | SDL.InitFlags.Audio)) return;
        Mixer.Init();
        TTF.Init();

        window = new Window(title, width, height, flags);
        device = new GPUDevice(window);
        renderer = new GPURenderer(window, device);
        ShaderCross.Init();

        mixer = new AudioMixer(SDL.AudioDeviceDefaultPlayback);
        pipeline2D = GraphicPipeline2D.CreatePipeline(window, device);
        fullscreenRenderer = new FullscreenRenderer(window, renderer);
        sprite2DBatcher = new Sprite2DBatcher(renderer);
        textRenderer = new GPUTextRendering(device);
    }

    public static void Dispose()
    {
        mixer.Dispose();
        fullscreenRenderer.Dispose();
        pipeline2D.Dispose();
        sprite2DBatcher.Dispose();
        device.Dispose();
        window.Dispose();

        ShaderCross.Quit();
        Mixer.Quit();
        SDL.Quit();
    }

    public static void RenderTarget(RenderTarget? target)
    {
        renderer.SetRenderTarget(target);
    }

    public static void ClearScreen(Vec4 color)
    {
        renderer.ClearScreen(color);
    }

    public static void BeginPass(GraphicsPipeline pipeline)
    {
        renderer.BeginPass(pipeline);
    }

    public static void EndPass()
    {
        renderer.EndPass();
    }

    public static void BeginDrawing()
    {
        renderer.AcquireSwapChain();
    }

    public static void EndDrawing()
    {
        renderer.Commit();
    }
}
