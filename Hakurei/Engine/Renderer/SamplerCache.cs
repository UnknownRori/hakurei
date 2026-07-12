namespace Hakurei.Engine.Renderer;

public class SamplerCache
{
    static public Sampler NearestFilter;

    static public void Init(GPUDevice device)
    {
        NearestFilter = new Sampler(device, Sampler.Filter.Nearest);
    }

    static public void Dispose()
    {
        NearestFilter.Dispose();
    }
}
