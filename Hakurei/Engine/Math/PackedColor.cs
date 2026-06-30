using System.Runtime.InteropServices;

namespace Hakurei.Engine.Math;

[StructLayout(LayoutKind.Sequential)]
public struct PackedColor
{
    public byte r = 0;
    public byte g = 0;
    public byte b = 0;
    public byte a = 0;

    public PackedColor(uint r, uint g, uint b, uint a)
    {
        this.r = (byte) r;
        this.g = (byte) g;
        this.b = (byte) b;
        this.a = (byte) a;
    }

    public PackedColor(Vec4 vec4)
    {
        this.r = (byte)(vec4.r * 255);
        this.g = (byte)(vec4.g * 255);
        this.b = (byte)(vec4.b * 255);
        this.a = (byte)(vec4.a * 255);
    }

    public static PackedColor WHITE()
    {
        return new PackedColor((byte)255, (byte)255, (byte)255, (byte)255);
    }


}
