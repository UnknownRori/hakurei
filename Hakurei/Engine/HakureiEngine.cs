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
        textRenderer.Dispose();
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

    /////////////
    // Resources
    /////////////

    public static Texture LoadTexture(string filename)
    {
        return new Texture(device, filename);
    }

    public static Texture LoadTexture(Hakurei.Engine.Renderer.Image image)
    {
        return new Texture(device, image);
    }

    public static Hakurei.Engine.Renderer.Image LoadImage(string filename)
    {
        return new Hakurei.Engine.Renderer.Image(filename);
    }

    public static Font LoadFont(string filename)
    {
        return new Font(filename, 12);
    }

    public static Font LoadFont(string filename, float pointSize)
    {
        return new Font(filename, pointSize);
    }

    public static RenderTarget LoadRenderTarget(uint width, uint height)
    {
        return new RenderTarget(device, width, height);
    }

    /////////////
    // Object
    /////////////

    public static Text CreateText(Font font, string text)
    {
        return TextRenderer.CreateText(font, text);
    }

    /////////////
    // Rendering
    /////////////

    public static void SetRenderTarget(RenderTarget? target)
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

    public static void BeginMode2D()
    {
        renderer.BeginPass(pipeline2D);
    }

    public static void EndMode2D()
    {
        sprite2DBatcher.Flush();
        renderer.EndPass();
    }

    public static void SetCamera2D(Camera2D? camera)
    {
        Camera2D cam = camera != null ? camera : new Camera2D();
        UniformBlock uni = new UniformBlock(cam.GetViewProjection(renderer.SwapChainWidth, renderer.SwapChainHeight));
        renderer.PushVertexUniform(uni);
    }

    public static void Blit(RenderTarget src, RenderTarget? dst)
    {
        fullscreenRenderer.Blit(src, dst);
    }

    public static void DrawText(Text text, Vec2 pos, PackedColor tint)
    {
        sprite2DBatcher.DrawText(text.Obj, pos, tint);
    }

    public static void Draw(Sprite2D sprite)
    {
        sprite2DBatcher.Draw(sprite);
    }

    public static void Draw(Texture texture, Vec2 position, PackedColor tint)
    {
        sprite2DBatcher.Draw(texture, position, tint);
    }

    public static void Draw(
        Texture texture,
        Vec2 position,
        float scale,
        float radiansRotation,
        PackedColor tint)
    {
        sprite2DBatcher.Draw(texture, position, scale, radiansRotation, tint);
    }

    public static void Draw(
        Texture texture,
        Rect srcRect,
        Rect dstRect,
        Vec2 origin,
        float radiansRotation,
        PackedColor tint
    )
    {
        sprite2DBatcher.Draw(texture, srcRect, dstRect, origin, radiansRotation, tint);
    }
}
