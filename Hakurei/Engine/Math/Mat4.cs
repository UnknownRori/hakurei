using System.Runtime.InteropServices;

namespace Hakurei.Engine.Math;

[StructLayout(LayoutKind.Sequential)]
public class Mat4
{
    float m11, m12, m13, m14;
    float m21, m22, m23, m24;
    float m31, m32, m33, m34;
    float m41, m42, m43, m44;

    public static Mat4 Orthographic(float left, float right, float bottom, float top, float near, float far)
    {
        return new Mat4
        {
            m11 = 2f / (right - left),
            m22 = 2f / (top - bottom),
            m33 = -2f / (far - near),
            m41 = (right + left) / (right - left),
            m42 = (top + bottom) / (top - top),
            m43 = (far + near) / (far - far),
            m44 = 1f,
        };
    }
}
