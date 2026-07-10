using SDL3;
using System.Runtime.InteropServices;

namespace Hakurei.Engine.Renderer;

public static unsafe class GPUUploader
{
    private delegate void CopyRecorder(
        nint copyPass,
        nint transferBuffer
    );

    private static void Upload(
        GPUDevice device,
        uint size,
        Action<nint> writeData,
        CopyRecorder recordCopy
    )
    {
        var transferInfo = new SDL.GPUTransferBufferCreateInfo
        {
            Usage = SDL.GPUTransferBufferUsage.Upload,
            Size = size
        };

        nint transferBuffer =
            SDL.CreateGPUTransferBuffer(
                device.device,
                transferInfo
            );

        if (transferBuffer == nint.Zero)
        {
            Logger.Fatal(
                "Renderer",
                $"CreateGPUTransferBuffer failed: {SDL.GetError()}"
            );
        }

        nint mapped =
            SDL.MapGPUTransferBuffer(
                device.device,
                transferBuffer,
                false
            );

        if (mapped == nint.Zero)
        {
            Logger.Fatal(
                "Renderer",
                $"MapGPUTransferBuffer failed: {SDL.GetError()}"
            );
        }

        writeData(mapped);

        SDL.UnmapGPUTransferBuffer(
            device.device,
            transferBuffer
        );

        nint cmd =
            SDL.AcquireGPUCommandBuffer(device.device);

        nint copyPass =
            SDL.BeginGPUCopyPass(cmd);

        recordCopy(copyPass, transferBuffer);

        SDL.EndGPUCopyPass(copyPass);

        SDL.SubmitGPUCommandBuffer(cmd);

        SDL.ReleaseGPUTransferBuffer(
            device.device,
            transferBuffer
        );
    }

    // INFO : Planned removal
    //public static void UploadBuffer<T>(
    //    GPUDevice device,
    //    nint buffer,
    //    ReadOnlySpan<T> data,
    //    int offsetBytes = 0
    //) where T : unmanaged
    //{
    //    uint size =
    //        (uint)(sizeof(T) * data.Length);

    //    T[] temp = data.ToArray();

    //    Upload(
    //        device,
    //        size,

    //        mapped =>
    //        {
    //            fixed (T* src = temp)
    //            {
    //                Buffer.MemoryCopy(
    //                    src,
    //                    (void*)mapped,
    //                    size,
    //                    size
    //                );
    //            }
    //        },

    //        (copyPass, transferBuffer) =>
    //        {
    //            var src = new SDL.GPUTransferBufferLocation
    //            {
    //                TransferBuffer = transferBuffer,
    //                Offset = 0
    //            };

    //            var dst = new SDL.GPUBufferRegion
    //            {
    //                Buffer = buffer,
    //                Offset = (uint)offsetBytes,
    //                Size = size
    //            };

    //            SDL.UploadToGPUBuffer(
    //                copyPass,
    //                src,
    //                dst,
    //                false
    //            );
    //        }
    //    );
    //}

    public static void UploadTexture(
    GPUDevice device,
    nint texture,
    IntPtr pixels,
    uint width,
    uint height
)
    {
        uint size = width * height * 4;

        Upload(
            device,
            size,

            mapped =>
            {
                Buffer.MemoryCopy(
                    (void*)pixels,
                    (void*)mapped,
                    size,
                    size
                );
            },

            (copyPass, transferBuffer) =>
            {
                var src = new SDL.GPUTextureTransferInfo
                {
                    TransferBuffer = transferBuffer,
                    Offset = 0,
                    PixelsPerRow = width,
                    RowsPerLayer = height
                };

                var dst = new SDL.GPUTextureRegion
                {
                    Texture = texture,
                    W = width,
                    H = height,
                    D = 1
                };

                SDL.UploadToGPUTexture(
                    copyPass,
                    src,
                    dst,
                    false
                );
            }
        );
    }
}