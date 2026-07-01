using Hakurei.Engine.Math;
using Hakurei.Engine.Renderer;
using SDL3;

namespace Hakurei.Engine.Renderer2D;

public class Sprite2DBatcher
{
    private GPURenderer renderer;
    private GPUBuffer<Vertex2D> vertexBuffer;
    static private GPUBuffer<UInt32> indicesBuffer;
    static private bool _sinit = false;
    private const int MAX_INSTANCE = 4096;

    private uint instanceCount = 0;
    private uint _capacity;
    private List<Vertex2D> vertex = new List<Vertex2D>();

    private Texture? _texture = null;

    public Sprite2DBatcher(GPURenderer renderer, uint maxInstance = 256)
    {
        this.renderer = renderer;
        _capacity = maxInstance;

        if (maxInstance > MAX_INSTANCE)
        {
            maxInstance = MAX_INSTANCE;
            Logger.Warn("Renderer2D", $"Sprite2DBatcher create GPUBuffer beyond {MAX_INSTANCE}");
        }

        vertexBuffer = new GPUBuffer<Vertex2D>(renderer.Device, SDL.GPUBufferUsageFlags.Vertex, (int)maxInstance * 4);
        if (!_sinit)
        {
            _sinit = true;
            List<UInt32> indices = new List<UInt32>();
            indicesBuffer = new GPUBuffer<uint>(renderer.Device, SDL.GPUBufferUsageFlags.Index, MAX_INSTANCE * 6);
            for (uint i = 0; i < MAX_INSTANCE; i++)
            {
                UInt32 baseCount = i * 4;
                indices.Add(baseCount);
                indices.Add(baseCount + 1);
                indices.Add(baseCount + 2);

                indices.Add(baseCount + 2);
                indices.Add(baseCount + 3);
                indices.Add(baseCount);
            }
            indicesBuffer.Upload(indices.ToArray());
        }
    }

    public void Flush()
    {
        if (_texture == null) throw new Exception("Texture must be binded first!");

        vertexBuffer.Upload(vertex.ToArray());

        renderer.BindTexture(_texture);
        renderer.BindVertex(vertexBuffer);
        renderer.BindIndices(indicesBuffer);

        renderer.DrawIndexed(instanceCount * 6);

        vertex.Clear();
        instanceCount = 0;
    }

    public void DrawSprite(Sprite2D sprite)
    {
        DrawSprite(sprite.Texture, sprite.Position, new PackedColor(sprite.Tint));
    }

    public void DrawSprite(Texture texture, Vec2 position, PackedColor tint)
    {
        DrawSpritePro(texture, position, new(1f), 0f, Vec2.Zero, new(0f, 0f, texture.Width, texture.Height), tint);
    }

    public void DrawSpritePro(Texture texture, Vec2 position, float scale, float radiansRotation, PackedColor tint)
    {
        DrawSpritePro(
            texture,
            position,
            new(scale),
            radiansRotation,
            new(texture.Width / 2f, texture.Height / 2f),
            new(0f, 0f, texture.Width, texture.Height),
            tint
        );
    }

    public void DrawSpritePro(
        Texture texture,
        Vec2 position,
        Vec2 scale,
        float radiansRotation,
        Vec2 origin,
        Rect region,
        PackedColor tint
    )
    {
        if (_texture != null && _texture.texture != texture.texture)
            Flush();
        if (instanceCount > _capacity)
            Flush();

        _texture = texture;

        float width = region.Width;
        float height = region.Height;

        UInt32 baseCount = (UInt32)vertex.Count;

        float cos = MathF.Cos(radiansRotation);
        float sin = MathF.Sin(radiansRotation);

        Vec2 tl = new Vec2(-origin.x, -origin.y);
        Vec2 tr = new Vec2(width - origin.x, -origin.y);
        Vec2 br = new Vec2(width - origin.x, height - origin.y);
        Vec2 bl = new Vec2(-origin.x, height - origin.y);

        tl *= scale;
        tr *= scale;
        br *= scale;
        bl *= scale;

        tl = Rotate(tl, cos, sin);
        tr = Rotate(tr, cos, sin);
        br = Rotate(br, cos, sin);
        bl = Rotate(bl, cos, sin);

        tl += position;
        tr += position;
        br += position;
        bl += position;

        float u0 = region.X / texture.Width;
        float v0 = region.Y / texture.Height;

        float u1 = (region.X + region.Width) / texture.Width;
        float v1 = (region.Y + region.Height) / texture.Height;

        vertex.Add(new Vertex2D
        {
            Position = new Vec3(tl.x, tl.y, 0f),
            UV = new Vec2(u0, v1),
            Tint = tint
        });

        vertex.Add(new Vertex2D
        {
            Position = new Vec3(tr.x, tr.y, 0f),
            UV = new Vec2(u1, v1),
            Tint = tint
        });

        vertex.Add(new Vertex2D
        {
            Position = new Vec3(br.x, br.y, 0f),
            UV = new Vec2(u1, v0),
            Tint = tint
        });

        vertex.Add(new Vertex2D
        {
            Position = new Vec3(bl.x, bl.y, 0f),
            UV = new Vec2(u0, v0),
            Tint = tint
        });

        instanceCount++;
    }

    private static Vec2 Rotate(Vec2 v, float cos, float sin)
    {
        return new Vec2(
            v.x * cos - v.y * sin,
            v.x * sin + v.y * cos
        );
    }
}
