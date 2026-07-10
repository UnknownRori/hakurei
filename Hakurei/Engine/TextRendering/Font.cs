using SDL3;

namespace Hakurei.Engine.TextRendering;

public class Font : IDisposable
{
    nint _font;

    public nint font {  get { return _font; } }

    public Font(string filename, float pointSize)
    {
        _font = TTF.OpenFont(filename, pointSize);
    }

    public void Dispose()
    {
        TTF.CloseFont(_font);
    }
}
