using SDL3;

namespace Hakurei.Engine.Renderer;

public class GPUDevice: IDisposable
{
    private nint _device;
    public nint device { get => _device; }

    public GPUDevice(Window window)
    {
        _device = SDL.CreateGPUDevice(SDL.GPUShaderFormat.SPIRV, true, null);
        if (!SDL.ClaimWindowForGPUDevice(_device, window.window))
        {
            Logger.Fatal("Renderer", "GPU Device failed to claim window");
        }
        Logger.Log("Renderer", "GPU Device successfully initialized");
    }

    public void Dispose()
    {
        if (_device == nint.Zero) return;
        Logger.Log("Renderer", "GPU Device successfully destroyed");
        SDL.DestroyGPUDevice(_device);
        _device = nint.Zero;
    }
}
