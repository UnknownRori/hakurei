using System.Runtime.InteropServices;

namespace Hakurei.Engine.Math;

[StructLayout(LayoutKind.Sequential)]
public struct Vec2
{
    public float x, y;

    public Vec2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
}