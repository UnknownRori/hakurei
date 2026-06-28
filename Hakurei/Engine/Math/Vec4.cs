using System.Runtime.InteropServices;

namespace Hakurei.Engine.Math;

[StructLayout(LayoutKind.Sequential)]
public struct Vec4
{
    public float x, y, z, w;

    public float r { get => x; set => x = value; }
    public float g { get => y; set => y = value; }
    public float b { get => z; set => z = value; }
    public float a { get => w; set => w = value; }

    public Vec4(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }
}