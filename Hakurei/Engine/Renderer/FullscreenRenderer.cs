using Hakurei.Engine.Math;
using SDL3;
using System;

namespace Hakurei.Engine.Renderer;

public class FullscreenRenderer : IDisposable
{
    GPURenderer _renderer;
    private GPUBuffer<Vertex> vertexBuffer;
    private GPUBuffer<UInt16> indicesBuffer;
    GraphicsPipeline fullscreenPipeline;

    static readonly Vertex[] verticesSwapChain = new Vertex[] {
        new () { Position = new Vec3(-1f, -1f, 0f), UV = new Vec2(0f, 0f) },
        new () { Position = new Vec3( 1f, -1f, 0f), UV = new Vec2(1f, 0f) },
        new () { Position = new Vec3( 1f,  1f, 0f), UV = new Vec2(1f, 1f) },
        new () { Position = new Vec3(-1f,  1f, 0f), UV = new Vec2(0f, 1f) },

        new () { Position = new Vec3(-1f, -1f, 0f), UV = new Vec2(0f, 1f) },
        new () { Position = new Vec3( 1f, -1f, 0f), UV = new Vec2(1f, 1f) },
        new () { Position = new Vec3( 1f,  1f, 0f), UV = new Vec2(1f, 0f) },
        new () { Position = new Vec3(-1f,  1f, 0f), UV = new Vec2(0f, 0f) },
    };

    static readonly UInt16[] indexes = new UInt16[] {
        0, 1, 2,
        2, 3, 0,

        4, 5, 6,
        6, 7, 4,
    };

    public FullscreenRenderer(Window window, GPURenderer renderer)
    {
        _renderer = renderer;
        vertexBuffer = new GPUBuffer<Vertex>(renderer.Device, SDL.GPUBufferUsageFlags.Vertex, verticesSwapChain.Length);
        indicesBuffer = new GPUBuffer<UInt16>(renderer.Device, SDL.GPUBufferUsageFlags.Index, indexes.Length);
        fullscreenPipeline = DefaultPipeline.CreatePipeline(window, renderer.Device, new DefaultShader(renderer.Device));

        vertexBuffer.Upload(verticesSwapChain);
        indicesBuffer.Upload(indexes);
    }

    public void Dispose()
    {
        vertexBuffer.Dispose(); 
        indicesBuffer.Dispose();
        fullscreenPipeline.Dispose();
    }

    public void Blit(RenderTarget src, RenderTarget? dst = null)
    {
        _renderer.SetRenderTarget(dst);
        _renderer.ClearScreen(Vec4.Black);
        _renderer.BeginPass(fullscreenPipeline);
            _renderer.BindVertex(vertexBuffer);
            _renderer.BindIndices(indicesBuffer);
            _renderer.BindTexture(src.Texture);
            if (dst == null) _renderer.DrawIndexed(6);
            else _renderer.DrawIndexed(6, 6);
        _renderer.EndPass();
    }
}
