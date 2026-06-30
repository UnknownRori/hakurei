using Hakurei.Engine.Math;
using Hakurei.Engine.Renderer;
using SDL3;
using System.Runtime.InteropServices;

namespace Hakurei.Engine.Renderer2D;

public class Sprite2DBatcher
{
    private GPURenderer renderer;
    private Texture texture;
    private GraphicsPipeline pipeline;
    private GPUBuffer<Vertex2D> vertexBuffer;
    private GPUBuffer<UInt32> indicesBuffer;

    private uint instanceCount = 0;
    private List<Vertex2D> vertex = new List<Vertex2D>();
    private List<UInt32> indices = new List<UInt32>();
    private Camera2D _camera;
    

    public Sprite2DBatcher(GPURenderer renderer, Texture texture, uint maxInstance = 256)
    {
        this.renderer = renderer;
        this.texture = texture;
        this.pipeline = GraphicPipeline2D.CreatePipeline(renderer.Window, renderer.Device);
        this._camera = new Camera2D();

        vertexBuffer = new GPUBuffer<Vertex2D>(renderer.Device, SDL.GPUBufferUsageFlags.Vertex, (int) maxInstance * 4);
        indicesBuffer = new GPUBuffer<uint>(renderer.Device, SDL.GPUBufferUsageFlags.Index, (int) maxInstance * 6);
    }

    public void BindCamera(Camera2D camera)
    {
        this._camera = camera;
    }

    public void PushSprite(Vec2 position, PackedColor tint)
    {
        var width = texture.Width;
        var height = texture.Height;
        var baseCount = (UInt32) vertex.Count();

        var vertices = new Vertex2D[]
        {
            new () { Position = new Vec3(position.x        , position.y         , 0f), UV = new Vec2(0f, 1f), Tint = tint },
            new () { Position = new Vec3(position.x + width, position.y         , 0f), UV = new Vec2(1f, 1f), Tint = tint },
            new () { Position = new Vec3(position.x + width, position.y + height, 0f), UV = new Vec2(1f, 0f), Tint = tint },
            new () { Position = new Vec3(position.x        , position.y + height, 0f), UV = new Vec2(0f, 0f), Tint = tint },
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

        UniformBlock uni = new UniformBlock(_camera.GetViewProjection(renderer.SwapChainWidth, renderer.SwapChainHeight));

        renderer.BeginPass(pipeline);
            renderer.PushVertexUniform(uni);
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
