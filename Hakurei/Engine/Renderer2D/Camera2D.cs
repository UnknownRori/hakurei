using Hakurei.Engine.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hakurei.Engine.Renderer2D;

public class Camera2D
{
    public Vec2 Position;
    public float Zoom = 1f;

    public Camera2D()
    {
        Position.x = 0;
        Position.y = 0;
    }

    public Mat4 GetViewProjection(uint screenWidth, uint screenHeight)
    {
        float left   = Position.x;
        float right  = Position.x + screenWidth / Zoom;
        float top    = Position.y;
        float bottom = Position.y + screenHeight / Zoom;
        return Mat4.Orthographic(left, right, bottom, top, -1f, 1f);
    }

}
