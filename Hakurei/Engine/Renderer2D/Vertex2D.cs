using Hakurei.Engine.Math;
using System.Runtime.InteropServices;

namespace Hakurei.Engine.Renderer2D;

[StructLayout(LayoutKind.Sequential)]
public struct Vertex2D
{
    public Vec3 Position;
    public Vec2 UV;
    public PackedColor Tint;
}