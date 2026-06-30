namespace Hakurei.Engine.Renderer;
using SDL3;
using static SDL3.SDL;

public class Texture : IDisposable
{
    private static uint imageId = 0;

    private uint _id;
    private nint _texture;
    private nint _sampler;
    private GPUDevice _device;

    public readonly uint Width;
    public readonly uint Height;

    public nint texture { get => _texture; }
    public nint sampler{ get => _sampler; }
    public uint id { get  => _id; }

    public Texture(GPUDevice device, string filename)
    {
        Image image = new Image(filename);
        _device = device;
        _texture = CreateGPUTexture(device, SDL.GPUTextureType.TextureType2D, SDL.GPUTextureUsageFlags.Sampler, image.width, image.height);
        _sampler = CreateSampler();
        Width = image.width;
        Height = image.height;
        Upload(image);

        image.Dispose();
        _id = imageId++;
        Logger.Log("Renderer", $"Texture created successfully ({_id})");
    }

    public Texture(GPUDevice device, Image image)
    {
        _device = device;
        _texture = CreateGPUTexture(
            device,
            SDL.GPUTextureType.TextureType2D, 
            SDL.GPUTextureUsageFlags.Sampler, 
            image.width, 
            image.height
        );
        _sampler = CreateSampler();
        Width = image.width;
        Height = image.height;
        Upload(image);

        _id = imageId++;
        Logger.Log("Renderer", $"Texture created successfully ({_id})");
    }

    public Texture(GPUDevice device, SDL.GPUTextureType type, SDL.GPUTextureUsageFlags usage, uint width, uint height)
    {
        _device = device;
        _texture = CreateGPUTexture(
            device, 
            type, 
            SDL.GPUTextureUsageFlags.Sampler | SDL.GPUTextureUsageFlags.ColorTarget, 
            width, 
            height
        );
        _sampler = CreateSampler();
        Width = width;
        Height = height;

        _id = imageId++;
        Logger.Log("Renderer", $"Texture created successfully ({_id})");
    }

    public void Dispose()
    {
        if (_texture == nint.Zero) return;
        SDL.ReleaseGPUTexture(_device.device, _texture);
        _texture = nint.Zero;
        Logger.Log("Renderer", $"Texture destroyed successfully ({_id})");
    }

    private static nint CreateGPUTexture(
        GPUDevice device, 
        SDL.GPUTextureType type, 
        SDL.GPUTextureUsageFlags usage, 
        uint width, 
        uint height
       )
    {
        var textureInfo = new SDL.GPUTextureCreateInfo
        {
            Type = type,
            Usage = usage,
            Format = SDL.GPUTextureFormat.R8G8B8A8Unorm,
            Width = width,
            Height = height,
            LayerCountOrDepth = 1,
            NumLevels = 1,
        };
        nint texture = SDL.CreateGPUTexture(device.device, textureInfo);
        if (texture == nint.Zero)
        {
            Logger.Fatal("Renderer", $"Failed to create texture: {SDL.GetError()}");
        }
        return texture;
    }

    public nint CreateSampler()
    {
        var samplerInfo = new SDL.GPUSamplerCreateInfo
        {
            MinFilter = SDL.GPUFilter.Nearest,
            MagFilter = SDL.GPUFilter.Nearest,

            MipmapMode = SDL.GPUSamplerMipmapMode.Nearest,

            AddressModeU = SDL.GPUSamplerAddressMode.Repeat,
            AddressModeV = SDL.GPUSamplerAddressMode.Repeat,
            AddressModeW = SDL.GPUSamplerAddressMode.Repeat
        };

        nint sampler =
            SDL.CreateGPUSampler(
                _device.device,
                samplerInfo
            );
        if (sampler == nint.Zero)
        {
            Logger.Fatal("Renderer", $"Failed to create sampler: {SDL.GetError()}");
        }
        return sampler;
    }
    private void Upload(Image 
        image)
    {
        GPUUploader.UploadTexture(
            _device,
            _texture,
            image.pixels,
            image.width,
            image.height
        );
    }
}
