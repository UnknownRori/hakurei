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

    public nint CreateText(Font font, string text)
    {
        nint obj = TTF.CreateText(_GPUTextEngine, font.font, text, (nuint) text.Length);
        return obj;
    }

    public void Dispose()
    {
        TTF.DestroyGPUTextEngine(_GPUTextEngine);
    }
}
