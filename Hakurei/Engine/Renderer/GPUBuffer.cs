using SDL3;
using System.Runtime.InteropServices;

namespace Hakurei.Engine.Renderer;

public unsafe class GPUBuffer<T> : IDisposable where T : unmanaged
{
    public nint Buffer { get; private set; }
    public int Capacity { get; private set; }

    private readonly GPUDevice device;
    private readonly SDL.GPUBufferUsageFlags usage;
    private readonly int stride;
    private readonly uint capacityBytes;
    private readonly nint transferBuffer;

    public GPUBuffer(
        GPUDevice device,
        SDL.GPUBufferUsageFlags usage,
        int capacity
    )
    {
        this.device = device;
        this.usage = usage;
        stride = Marshal.SizeOf<T>();
        Capacity = capacity;
        capacityBytes = (uint)(stride * capacity);

        var bufferInfo = new SDL.GPUBufferCreateInfo
        {
            Usage = usage,
            Size = capacityBytes
        };

        Buffer = SDL.CreateGPUBuffer(device.device, bufferInfo);
        if (Buffer == nint.Zero)
            Logger.Fatal("Renderer", $"CreateGPUBuffer failed: {SDL.GetError()}");

        var transferInfo = new SDL.GPUTransferBufferCreateInfo
        {
            Usage = SDL.GPUTransferBufferUsage.Upload,
            Size = capacityBytes
        };

        transferBuffer = SDL.CreateGPUTransferBuffer(device.device, transferInfo);
        if (transferBuffer == nint.Zero)
            Logger.Fatal("Renderer", $"CreateGPUTransferBuffer failed: {SDL.GetError()}");
    }

    public void Upload(T[] data, int offsetElements = 0)
    {
        if (data.Length == 0)
            return;

        if (offsetElements + data.Length > Capacity)
        {
            Logger.Fatal("Renderer", "GPUBuffer: Upload exceeds capacity");
            return;
        }

        int offsetBytes = stride * offsetElements;
        uint sizeBytes = (uint)(stride * data.Length);

        nint mapped = SDL.MapGPUTransferBuffer(device.device, transferBuffer, true);
        if (mapped == nint.Zero)
        {
            Logger.Fatal("Renderer", $"MapGPUTransferBuffer failed: {SDL.GetError()}");
            return;
        }

        fixed (T* src = data)
        {
            System.Buffer.MemoryCopy(src, (void*)mapped, sizeBytes, sizeBytes);
        }

        SDL.UnmapGPUTransferBuffer(device.device, transferBuffer);

        nint cmd = SDL.AcquireGPUCommandBuffer(device.device);
        nint copyPass = SDL.BeginGPUCopyPass(cmd);

        var src2 = new SDL.GPUTransferBufferLocation
        {
            TransferBuffer = transferBuffer,
            Offset = 0
        };

        var dst = new SDL.GPUBufferRegion
        {
            Buffer = Buffer,
            Offset = (uint)offsetBytes,
            Size = sizeBytes
        };

        SDL.UploadToGPUBuffer(copyPass, src2, dst, true);

        SDL.EndGPUCopyPass(copyPass);
        SDL.SubmitGPUCommandBuffer(cmd);
    }

    public void Dispose()
    {
        if (transferBuffer != nint.Zero)
            SDL.ReleaseGPUTransferBuffer(device.device, transferBuffer);

        if (Buffer != nint.Zero)
        {
            SDL.ReleaseGPUBuffer(device.device, Buffer);
            Buffer = nint.Zero;
        }
    }
}