using Hakurei.Engine.Math;
using Hakurei.Engine.Renderer;
using SDL3;

namespace Hakurei.Engine.Renderer2D;

public class Sprite2DBatcher
{
    private GPURenderer renderer;
    private GPUBuffer<Vertex2D> vertexBuffer;
    private GPUBuffer<UInt32> indicesBuffer;

    private uint instanceCount = 0;
    private List<Vertex2D> vertex = new List<Vertex2D>();
    private List<UInt32> indices = new List<UInt32>();

    private Texture? _texture = null;

    public Sprite2DBatcher(GPURenderer renderer, uint maxInstance = 256)
    {
        this.renderer = renderer;

        vertexBuffer = new GPUBuffer<Vertex2D>(renderer.Device, SDL.GPUBufferUsageFlags.Vertex, (int)maxInstance * 4);
        indicesBuffer = new GPUBuffer<uint>(renderer.Device, SDL.GPUBufferUsageFlags.Index, (int)maxInstance * 6);
    }

    public void Flush()
    {
        if (_texture == null) throw new Exception("Texture must be binded first!");

        vertexBuffer.Upload(vertex.ToArray());
        indicesBuffer.Upload(indices.ToArray());

        renderer.BindTexture(_texture);
        renderer.BindVertex(vertexBuffer);
        renderer.BindIndices(indicesBuffer);

        renderer.DrawIndexed(instanceCount * 6);

        indices.Clear();
        vertex.Clear();
        instanceCount = 0;
    }

    public void DrawSprite(Sprite2D sprite)
    {
        DrawSprite(sprite.Texture, sprite.Position, new PackedColor(sprite.Tint));
    }

    public void DrawSprite(Texture texture, Vec2 position, PackedColor tint)
    {
        if (_texture != null && _texture.texture != texture.texture)
            Flush();

        _texture = texture;
        var width = texture.Width;
        var height = texture.Height;
        var baseCount = (UInt32)vertex.Count();

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
}
