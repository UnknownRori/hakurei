using Hakurei.Engine.Math;
using System.Runtime.InteropServices;

namespace Hakurei.Engine.Renderer;

[StructLayout(LayoutKind.Sequential)]
public struct VertexBuffer
{
    public Vec3 Position;
    public Vec2 UV;
}