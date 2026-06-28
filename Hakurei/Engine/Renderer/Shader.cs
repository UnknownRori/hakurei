namespace Hakurei.Engine.Renderer;

using SDL3;

public class Shader : IDisposable
{
    private static uint shaderId = 0;

    private uint _id;
    private nint _shader;

    public nint shader { get => _shader;  }

    public Shader(ShaderBuilder builder)
    {
        _id= shaderId++;
        _shader = builder.Compile();
        Logger.Log("Renderer", $"Shader successfully compiled {_id}");
    }

    public void Dispose()
    {
        if (_shader == 0) return;
        SDL.Free(_shader);
        _shader = nint.Zero;
        Logger.Log("Renderer", $"Shader {_id} destroyed");
    }
}
