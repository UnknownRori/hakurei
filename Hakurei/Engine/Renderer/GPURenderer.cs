using Hakurei.Engine.Math;
using SDL3;
using System.Runtime.InteropServices;

namespace Hakurei.Engine.Renderer;

public class GPURenderer
{
    private GPUDevice _device;
    private Window _window;

    private nint _colorTargetPtr = nint.Zero;

    private nint _cmdBuf = nint.Zero;
    private nint _renderPass = nint.Zero;

    private nint _swapChainTexture = nint.Zero;
    private uint _swapChainWidth;
    private uint _swapChainHeight;

    public GPURenderer(Window window, GPUDevice device)
    {
        _device = device;
        _window = window;
    }

    public void AcquireSwapChain()
    {
        _cmdBuf = SDL.AcquireGPUCommandBuffer(_device.device);
        SDL.WaitAndAcquireGPUSwapchainTexture(_cmdBuf, _window.window, out _swapChainTexture, out _swapChainWidth, out _swapChainHeight);
    }

    public void BeginPass(GraphicsPipeline pipeline)
    {
        if (_colorTargetPtr == nint.Zero)
        {
            ClearScreen(new Vec4(0.1f, 0.1f, 0.15f, 1f));
        }

        _renderPass = SDL.BeginGPURenderPass(_cmdBuf, _colorTargetPtr, 1, nint.Zero);
        SDL.BindGPUGraphicsPipeline(_renderPass, pipeline.pipeline);
    }

    // TODO : Not sure if this is the correct way to bind vertex buffers, but it works for now.
    // Next will be to implement a proper vertex buffer binding system that can handle multiple buffers and attributes.
    public void BindVertex<T>(GPUBuffer<T> vertexBuffer) where T : unmanaged
    {
        SDL.GPUBufferBinding binding = new()
        {
            Buffer = vertexBuffer.Buffer,
            Offset = 0
        };

        SDL.BindGPUVertexBuffers(
            _renderPass,
            0,
           [binding],
            1
        );
    }

    public void BindIndices(GPUBuffer<UInt32> indicesBuffer)
    {
        SDL.GPUBufferBinding binding = new()
        {
            Buffer = indicesBuffer.Buffer,
            Offset = 0
        };
        

        SDL.BindGPUIndexBuffer(
            _renderPass,
           binding,
           SDL.GPUIndexElementSize.IndexElementSize32Bit
        );
    }

    public void BindTexture(Texture texture)
    {
        var binding = new SDL.GPUTextureSamplerBinding
        {
            Texture = texture.texture,
            Sampler = texture.sampler
        };
        SDL.BindGPUFragmentSamplers(_renderPass, 0, [binding], 1);
    }

    // TODO : Not sure if this is the correct way to draw, but it works for now.
    // Next will be to implement a proper draw call system that can handle multiple draw calls.
    public void Draw(uint verticesCount)
    {
        SDL.DrawGPUPrimitives(_renderPass, verticesCount, 1, 0, 0);
    }

    public void DrawIndexed(uint numIndices)
    {
        SDL.DrawGPUIndexedPrimitives(_renderPass, numIndices, 1, 0, 0, 0);
    }

    public void EndPass()
    {
        SDL.EndGPURenderPass(_renderPass);
        _renderPass = nint.Zero; _swapChainTexture = nint.Zero;
    }

    public void Commit()
    {
        if (!SDL.SubmitGPUCommandBuffer(_cmdBuf))
        {
            Logger.Fatal("Renderer", $"SubmitGPUCommandBuffer failed: {SDL.GetError()}");
        }

        _cmdBuf = nint.Zero;
        if (_colorTargetPtr != nint.Zero)
        {
            Marshal.FreeHGlobal(_colorTargetPtr);
            _colorTargetPtr = nint.Zero;
        }
    }

    public void ClearScreen(Vec4 color)
    {
        if (_colorTargetPtr != nint.Zero)
        {
            Marshal.FreeHGlobal(_colorTargetPtr);
            _colorTargetPtr = nint.Zero;
        }

        var colorTargetInfo = new SDL.GPUColorTargetInfo
        {
            Texture = _swapChainTexture,
            LoadOp = SDL.GPULoadOp.Clear,
            StoreOp = SDL.GPUStoreOp.Store,
            ClearColor = new SDL.FColor { R = color.r, G = color.g, B = color.b, A = color.a }
        };

        _colorTargetPtr = SDL.StructureToPointer<SDL.GPUColorTargetInfo>(colorTargetInfo);

    }
}
