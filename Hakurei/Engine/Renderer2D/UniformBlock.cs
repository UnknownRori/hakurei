using Hakurei.Engine.Math;
using System.Runtime.InteropServices;

namespace Hakurei.Engine.Renderer2D;

[StructLayout(LayoutKind.Sequential)]
internal class UniformBlock
{
    public Vec4 tint;
    public Mat4 mvp;

    public UniformBlock(Vec4 tint,  Mat4 mvp)
    {
        this.tint = tint;
        this.mvp  = mvp;
    }
}
