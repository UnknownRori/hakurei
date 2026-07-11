using SDL3;

namespace Hakurei.Engine.TextRendering;

public class Text: IDisposable
{
    nint obj;

    public nint Obj {  get { return obj; } }

    public Text(nint text)
    {
        obj = text;
    }

    public void Dispose()
    {
        TTF.DestroyText(obj);
    }
}
