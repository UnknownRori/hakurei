using SDL3;
using System.Runtime.InteropServices;

namespace Hakurei.Engine.Renderer;

public class GPUBuffer<T> where T : unmanaged
{
    public nint Buffer { get; private set; }

    public int Capacity { get; private set; }

    private readonly GPUDevice device;
    private readonly SDL.GPUBufferUsageFlags usage;

    private readonly int stride;

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

        int sizeInBytes = stride * capacity;

        var bufferInfo = new SDL.GPUBufferCreateInfo
        {
            Usage = usage,
            Size = (uint)sizeInBytes
        };

        Buffer = SDL.CreateGPUBuffer(device.device, bufferInfo);

        if (Buffer == nint.Zero)
            Logger.Fatal("Renderer", $"CreateGPUBuffer failed: {SDL.GetError()}");
    }

    public void Upload(T[] data, int offsetElements = 0)
    {
        if (offsetElements + data.Length > Capacity)
        {
            Logger.Fatal(
                "Renderer",
                "GPUBuffer: Upload exceeds capacity"
            );
        }

        int offsetBytes = stride * offsetElements;

        GPUUploader.UploadBuffer<T>(
            device,
            Buffer,
            data,
            offsetBytes
        );
    }

    public void Dispose()
    {
        if (Buffer != nint.Zero)
        {
            SDL.ReleaseGPUBuffer(
                device.device,
                Buffer
            );

            Buffer = nint.Zero;
        }
    }
}