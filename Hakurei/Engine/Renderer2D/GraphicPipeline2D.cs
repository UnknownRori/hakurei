using Hakurei.Engine.Renderer;
using SDL3;

namespace Hakurei.Engine.Renderer2D;

internal class GraphicPipeline2D
{
    public static GraphicsPipeline CreatePipeline(Window window, GPUDevice device)
    {
        LoadShader(device, out var vert, out var frag);
        GraphicsPipelineBuilder<Vertex> builder = new GraphicsPipelineBuilder<Vertex>(frag, vert);
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

    public static void LoadShader(GPUDevice device, out Shader vert, out Shader frag)
    {
        string vertSrc = File.ReadAllText("Engine/Shaders/2D.vert.hlsl");
        ShaderBuilder vertBuilder = new ShaderBuilder(device, ShaderCross.ShaderStage.Vertex, vertSrc);
        vert = vertBuilder.Build();

        string fragSrc = File.ReadAllText("Engine/Shaders/2D.frag.hlsl");
        ShaderBuilder fragBuilder = new ShaderBuilder(device, ShaderCross.ShaderStage.Fragment, fragSrc);
        frag = fragBuilder.Build();
    }
}
