using SDL3;
using System.Security.Cryptography;
using static System.Net.Mime.MediaTypeNames;
namespace Hakurei.Engine.Renderer;

public class RenderTarget : IDisposable
{
    private static uint imageId = 0;

    private uint _id;
    private Texture _texture;

    public Texture Texture {  get { return _texture; } }

    public RenderTarget(GPUDevice device, uint width, uint height)
    {
        _texture = new Texture(
            device, 
            SDL.GPUTextureType.TextureType2D, 
            SDL.GPUTextureUsageFlags.ColorTarget | SDL.GPUTextureUsageFlags.Sampler, 
            width, 
            height
        );

        _id = imageId++;
        Logger.Log("Renderer", $"RenderTarget created successfully ({_id}) on Texture({_texture.id})");
    }

    public void Dispose()
    {
        _texture.Dispose();
    }
}
