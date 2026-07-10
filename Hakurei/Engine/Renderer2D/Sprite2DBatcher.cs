using Hakurei.Engine.Math;
using Hakurei.Engine.Renderer;
using SDL3;

namespace Hakurei.Engine.Renderer2D;

public class Sprite2DBatcher : IDisposable
{
    private GPURenderer renderer;
    private GPUBuffer<Vertex2D> vertexBuffer;
    static private GPUBuffer<UInt16> indicesBuffer;
    static private bool _sinit = false;
    private const int MAX_INSTANCE = 4096;

    private uint instanceCount = 0;
    private List<Vertex2D> vertex = new List<Vertex2D>();

    private Texture? _texture = null;
    private nint _textSampler;

    public Sprite2DBatcher(GPURenderer renderer)
    {
        this.renderer = renderer;

        vertexBuffer = new GPUBuffer<Vertex2D>(renderer.Device, SDL.GPUBufferUsageFlags.Vertex, MAX_INSTANCE * 4);
        if (!_sinit)
        {
            _sinit = true;
            List<UInt16> indices = new List<UInt16>();
            indicesBuffer = new GPUBuffer<UInt16>(renderer.Device, SDL.GPUBufferUsageFlags.Index, MAX_INSTANCE * 6);
            for (uint i = 0; i < MAX_INSTANCE; i++)
            {
                UInt16 baseCount = (UInt16)(i * 4);
                indices.Add(baseCount);
                indices.Add((UInt16)(baseCount + 1));
                indices.Add((UInt16)(baseCount + 2));

                indices.Add((UInt16)(baseCount + 2));
                indices.Add((UInt16)(baseCount + 3));
                indices.Add(baseCount);
            }
            indicesBuffer.Upload(indices.ToArray());
        }

        var samplerInfo = new SDL.GPUSamplerCreateInfo
        {
            MinFilter = SDL.GPUFilter.Nearest,
            MagFilter = SDL.GPUFilter.Nearest,

            MipmapMode = SDL.GPUSamplerMipmapMode.Nearest,

            AddressModeU = SDL.GPUSamplerAddressMode.Repeat,
            AddressModeV = SDL.GPUSamplerAddressMode.Repeat,
            AddressModeW = SDL.GPUSamplerAddressMode.Repeat
        };

        _textSampler =
            SDL.CreateGPUSampler(
                renderer.Device.device,
                samplerInfo
            );
    }

    public void Dispose()
    {
        vertexBuffer.Dispose();
    }
    public static void Quit()
    {
        indicesBuffer.Dispose();
        _sinit = false;
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

    public void Draw(Sprite2D sprite)
    {
        Draw(sprite.Texture, sprite.Position, new PackedColor(sprite.Tint));
    }

    public void Draw(Texture texture, Vec2 position, PackedColor tint)
    {
        Draw(
            texture,
            new Rect(0, 0, texture.Width, texture.Height),
            new Rect(position.x, position.y, texture.Width, texture.Height),
            Vec2.Zero,
            0f,
            tint
        );
    }

    public void Draw(
        Texture texture,
        Vec2 position,
        float scale,
        float radiansRotation,
        PackedColor tint)
    {
        float width = texture.Width * scale;
        float height = texture.Height * scale;

        Draw(
            texture,
            new Rect(0, 0, texture.Width, texture.Height),
            new Rect(position.x, position.y, width, height),
            new Vec2(width / 2f, height / 2f),
            radiansRotation,
            tint
        );
    }

    public void Draw(
        Texture texture,
        Rect srcRect,
        Rect dstRect,
        Vec2 origin,
        float radiansRotation,
        PackedColor tint
    )
    {
        if (_texture != null && _texture.texture != texture.texture)
            Flush();

        if (instanceCount >= MAX_INSTANCE)
            Flush();

        _texture = texture;

        UInt32 baseCount = (UInt32)vertex.Count;

        float cos = MathF.Cos(radiansRotation);
        float sin = MathF.Sin(radiansRotation);

        // Destination size
        float width = dstRect.Width;
        float height = dstRect.Height;

        // Local corners relative to origin
        Vec2 tl = new Vec2(-origin.x, -origin.y);
        Vec2 tr = new Vec2(width - origin.x, -origin.y);
        Vec2 br = new Vec2(width - origin.x, height - origin.y);
        Vec2 bl = new Vec2(-origin.x, height - origin.y);

        // Rotate
        tl = tl.Rotate(cos, sin);
        tr = tr.Rotate(cos, sin);
        br = br.Rotate(cos, sin);
        bl = bl.Rotate(cos, sin);

        // Translate to destination position
        Vec2 position = new Vec2(dstRect.X, dstRect.Y);

        tl += position;
        tr += position;
        br += position;
        bl += position;

        // UVs from source rect
        float u0 = srcRect.X / texture.Width;
        float v0 = srcRect.Y / texture.Height;

        float u1 = (srcRect.X + srcRect.Width) / texture.Width;
        float v1 = (srcRect.Y + srcRect.Height) / texture.Height;

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

    public void DrawText(nint text, Vec2 position, PackedColor tint)
    {
        unsafe
        {
            TTF.GPUAtlasDrawSequence* seq = (TTF.GPUAtlasDrawSequence*)TTF.GetGPUTextDrawData(text);
            if (seq == null)
            {
                Logger.Fatal("Sprite2DBatcher", $"SDL error: {SDL.GetError()}");
            }
            for (; seq != null; seq = (TTF.GPUAtlasDrawSequence*)seq->Next)
            {
                var atlas = new Texture(seq->AtlasTexture, _textSampler);

                if (_texture != null && _texture.texture != atlas.texture)
                    Flush();
                if (instanceCount + (seq->NumVertices / 4) > MAX_INSTANCE)
                    Flush();

                _texture = atlas;
                var xyPtr = (SDL.FPoint*)seq->XY;
                var uvPtr = (SDL.FPoint*)seq->UV;
                var idxPtr = (int*)seq->Indices;

                for (int i = 0; i < seq->NumVertices; i++)
                {
                    var xy = xyPtr[i];
                    var uv = uvPtr[i];
                    vertex.Add(new Vertex2D
                    {
                        Position = new Vec3(xy.X + position.x, xy.Y + position.y, 0f),
                        UV = new Vec2(uv.X, uv.Y),
                        Tint = tint
                    });
                }
                instanceCount += (uint)(seq->NumVertices / 4);
            }
        }
    }
}
