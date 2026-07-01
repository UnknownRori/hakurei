using Hakurei.Engine.Math;
using Hakurei.Engine.Renderer;

namespace Hakurei.Engine.Renderer2D;

public class Sprite2D
{
    public Texture Texture;
    public Vec2 Position;
    public float Rotation;
    public float Scale;
    public Vec4 Tint;

    public Sprite2D(
        Texture texture,
        Vec2 position,
        Vec4 tint,
        float rotation = 0f,
        float scale = 1f
    )
    {
        Texture = texture;
        Position = position;
        Rotation = rotation;
        Scale = scale;
        Tint = tint;
    }
}
