namespace Hakurei.Engine.Renderer;
using SDL3;

public class Image: IDisposable
{
    private static uint imageId = 0;

    private uint _id;
    private nint _surface = nint.Zero;
    private uint _width;
    private uint _height;
    private int  _pitch;
    private nint _pixels;
    private SDL.PixelFormat _format;

    public nint surface { get => _surface; }
    public uint width { get => _width; }
    public uint height { get => _height; }
    public int  pitch { get => _pitch; }
    public nint pixels { get => _pixels; }
    public SDL.PixelFormat format { get => _format; }

    public Image(string filename)
    {
        nint loaded = SDL.LoadSurface(filename);
        if (loaded == nint.Zero)
        {
            Logger.Fatal("Renderer", $"Failed to load image: {SDL.GetError()}");
        }

        _surface = SDL.ConvertSurface(loaded, SDL.PixelFormat.ABGR8888);
        SDL.DestroySurface(loaded);

        if (_surface == nint.Zero)
        {
            Logger.Fatal("Renderer", $"Failed to convert image: {SDL.GetError()}");
        }


        unsafe
        {
            SDL.Surface* surface = (SDL.Surface*)_surface;
            _width = (uint)surface->Width;
            _height = (uint)surface->Height;
            _format = surface->Format;
            _pitch = surface->Pitch;
            _pixels = surface->Pixels;
        }

        _id = imageId++;
        Logger.Log("Renderer", $"Image loaded successfully ({_id}): {filename}");
    }

    public void Dispose()
    {
        if (_surface == nint.Zero) return;
        Logger.Log("Renderer", $"Image destroyed ({_id})");
        SDL.DestroySurface(_surface);
    }
}
