using SDL3;

namespace Hakurei.Engine.Renderer;

public class GraphicsPipeline: IDisposable
{
    private static uint graphicsId = 0;

    private GPUDevice _device;
    private uint _id;
    private nint _pipeline;

    public nint pipeline { get => _pipeline; }

    public GraphicsPipeline(GPUDevice device, nint pipeline)
    {
        _pipeline = pipeline;
        _device = device;
        _id = graphicsId++;
        Logger.Log("Renderer", $"Pipeline successfully created {_id}");
    }

    public void Dispose()
    {
        if (_pipeline == nint.Zero) return;
        SDL.ReleaseGPUGraphicsPipeline(_device.device, _pipeline);
        Logger.Log("Renderer", $"Pipeline {_id} destroyed ");
        _pipeline = nint.Zero;
    }
}
