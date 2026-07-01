using System.Runtime.InteropServices;

namespace Hakurei.Engine.Math;

[StructLayout(LayoutKind.Sequential)]
public struct Vec2
{
    public float x;
    public float y;

    public static readonly Vec2 Zero = new(0f, 0f);
    public static readonly Vec2 One = new(1f, 1f);
    public static readonly Vec2 Up = new(0f, 1f);
    public static readonly Vec2 Down = new(0f, -1f);
    public static readonly Vec2 Left = new(-1f, 0f);
    public static readonly Vec2 Right = new(1f, 0f);

    public Vec2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public Vec2(float xy)
    {
        this.x = xy;
        this.y = xy;
    }

    public readonly float Length()
    {
        return MathF.Sqrt(x * x + y * y);
    }

    public readonly float LengthSquared()
    {
        return x * x + y * y;
    }

    public readonly Vec2 Normalized()
    {
        float len = Length();

        if (len <= 0f)
            return Zero;

        return new Vec2(x / len, y / len);
    }

    public readonly Vec2 Rotated(float radians)
    {
        float cos = MathF.Cos(radians);
        float sin = MathF.Sin(radians);

        return new Vec2(
            x * cos - y * sin,
            x * sin + y * cos
        );
    }

    public static float Dot(Vec2 a, Vec2 b)
    {
        return a.x * b.x + a.y * b.y;
    }

    public static Vec2 operator +(Vec2 a, Vec2 b)
    {
        return new Vec2(a.x + b.x, a.y + b.y);
    }

    public static Vec2 operator -(Vec2 a, Vec2 b)
    {
        return new Vec2(a.x - b.x, a.y - b.y);
    }

    public static Vec2 operator *(Vec2 a, float scalar)
    {
        return new Vec2(a.x * scalar, a.y * scalar);
    }

    public static Vec2 operator *(Vec2 a, Vec2 b)
    {
        return new Vec2(a.x * b.x, a.y * b.y);
    }

    public static Vec2 operator /(Vec2 a, float scalar)
    {
        return new Vec2(a.x / scalar, a.y / scalar);
    }

    public override readonly string ToString()
    {
        return $"Vec2({x}, {y})";
    }
}
