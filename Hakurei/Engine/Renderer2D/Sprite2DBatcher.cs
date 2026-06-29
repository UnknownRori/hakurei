using Hakurei.Engine.Math;
using Hakurei.Engine.Renderer;
using SDL3;

namespace Hakurei.Engine.Renderer2D;

public class Sprite2DBatcher
{
    private GPURenderer renderer;
    private Texture texture;
    private GraphicsPipeline pipeline;
    private GPUBuffer<Vertex> vertexBuffer;
    private GPUBuffer<UInt32> indicesBuffer;

    private uint instanceCount = 0;
    private List<Vertex> vertex = new List<Vertex>();
    private List<UInt32> indices = new List<UInt32>();

    public Vec4 tint = new Vec4(1.0f, 1.0f, 1.0f, 1.0f);

    public Sprite2DBatcher(GPURenderer renderer, Texture texture)
    {
        this.renderer = renderer;
        this.texture = texture;
        this.pipeline = GraphicPipeline2D.CreatePipeline(renderer.Window, renderer.Device);

        SetupBuffer();
    }

    private void SetupBuffer()
    {
        vertexBuffer = new GPUBuffer<Vertex>(renderer.Device, SDL.GPUBufferUsageFlags.Vertex, 128);
        indicesBuffer = new GPUBuffer<uint>(renderer.Device, SDL.GPUBufferUsageFlags.Index, 128);
    }

    public void PushSprite(Vec2 position)
    {
        var width = 1f;
        var height = 1f;
        var baseCount = (UInt32) vertex.Count();
        var vertices = new Vertex[]
        {
            new () { Position = new Vec3(position.x        , position.y         , 0f), UV = new Vec2(0f, 1f) },
            new () { Position = new Vec3(position.x + width, position.y         , 0f), UV = new Vec2(1f, 1f) },
            new () { Position = new Vec3(position.x + width, position.y + height, 0f), UV = new Vec2(1f, 0f) },
            new () { Position = new Vec3(position.x        , position.y + height, 0f), UV = new Vec2(0f, 0f) },
        };
        var indexes = new UInt32[]
        {
            baseCount,
            baseCount + 1,
            baseCount + 2,
            baseCount + 2,
            baseCount + 3,
            baseCount,
            
        };
        vertex.AddRange(vertices);
        indices.AddRange(indexes);
        instanceCount += 1;
    }

    public void Submit()
    {
        vertexBuffer.Upload(vertex.ToArray());
        indicesBuffer.Upload(indices.ToArray());

        renderer.BeginPass(pipeline);
            renderer.PushVertexUniform(tint);
            renderer.BindTexture(texture);
            renderer.BindVertex(vertexBuffer);
            renderer.BindIndices(indicesBuffer);
            // TODO : Instance Draw
            while (instanceCount > 0)
            {
                renderer.DrawIndexed(6, (instanceCount - 1) * 6);
                instanceCount--;
            }
        renderer.EndPass();

        indices.Clear();
        vertex.Clear();
        instanceCount = 0;
    }
}
