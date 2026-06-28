namespace Hakurei.Engine.Renderer;

using SDL3;

public class DefaultPipeline
{
    public static GraphicsPipeline CreatePipeline(Window window, GPUDevice device, DefaultShader shd)
    {
        GraphicsPipelineBuilder<Vertex> builder = new GraphicsPipelineBuilder<Vertex>(shd.frag, shd.vert);
        builder.vertexAttributes.Add(new SDL.GPUVertexAttribute
        {
            Format = SDL.GPUVertexElementFormat.Float3,
            Offset = 0,
            BufferSlot = 0,
            Location = 0,
        });

        builder.vertexAttributes.Add(new SDL.GPUVertexAttribute
        {
            Format = SDL.GPUVertexElementFormat.Float2,
            Offset = 12,
            BufferSlot = 0,
            Location = 1,
        });

        return builder.Build(window, device);
    }
}
