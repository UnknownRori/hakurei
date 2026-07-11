using Hakurei.Engine.Math;
using Hakurei.Engine.Renderer;
using SDL3;

namespace Hakurei.Engine.TextRendering;

public class GPUTextRendering : IDisposable
{
    nint _GPUTextEngine;
    public GPUTextRendering(GPUDevice device)
    {
        _GPUTextEngine = TTF.CreateGPUTextEngine(device.device);
    }

    public Text CreateText(Font font, string text)
    {
        nint obj = TTF.CreateText(_GPUTextEngine, font.font, text, (nuint) text.Length);
        if (obj == nint.Zero)
        {
            Logger.Fatal("TextRenderer", $"Failed to create Text Object: {SDL.GetError()}");
        }
        return new(obj);
    }

    public void Dispose()
    {
        TTF.DestroyGPUTextEngine(_GPUTextEngine);
    }
}
