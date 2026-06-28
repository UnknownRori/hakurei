using System.Runtime.InteropServices;

namespace Hakurei.Engine.Math;

[StructLayout(LayoutKind.Sequential)]
public struct Vec3
{
    public float x, y, z;

    public Vec3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}