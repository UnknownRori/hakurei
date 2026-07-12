using SDL3;

namespace Hakurei.Engine.Renderer;

public class Sampler : IDisposable
{
    public enum Filter
    {
        Nearest,
        Linear,
    }

    private nint _sampler;
    private GPUDevice _device;

    public nint sampler { get { return _sampler; } }

    public Sampler(GPUDevice device, nint sampler)
    {
        _sampler = sampler;
        _device = device;
    }

    public Sampler(GPUDevice device, Filter filter)
    {
        _device = device;
        var sdlFilter = GetSDLFilter(filter);

        var samplerInfo = new SDL.GPUSamplerCreateInfo
        {
            MinFilter = SDL.GPUFilter.Nearest,
            MagFilter = SDL.GPUFilter.Nearest,

            MipmapMode = SDL.GPUSamplerMipmapMode.Nearest,

            AddressModeU = SDL.GPUSamplerAddressMode.Repeat,
            AddressModeV = SDL.GPUSamplerAddressMode.Repeat,
            AddressModeW = SDL.GPUSamplerAddressMode.Repeat
        };

        _sampler =
            SDL.CreateGPUSampler(
                device.device,
                samplerInfo
            );
        if (sampler == nint.Zero)
        {
            Logger.Fatal("Sampler", $"Failed to create sampler: {SDL.GetError()}");
        }
    }

    public void Dispose()
    {
        SDL.ReleaseGPUSampler(_device.device, _sampler);
    }

    private static SDL.GPUFilter GetSDLFilter(Filter filter)
    {
        switch (filter)
        {
            case Filter.Nearest:
                return SDL.GPUFilter.Nearest;
            case Filter.Linear:
                return SDL.GPUFilter.Linear;
        }
        return SDL.GPUFilter.Nearest;
    }
}
