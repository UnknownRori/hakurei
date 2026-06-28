using SDL3;
using System.Runtime.InteropServices;

namespace Hakurei.Engine.Renderer;

public class ShaderBuilder
{
    private readonly GPUDevice _device;

    public readonly string src;
    public string entryPoint = "main";
    public ShaderCross.ShaderStage stage;
    public uint numUniform = 0;
    public uint numSamplers = 0;
    public uint numStorageTextures = 0;
    public uint numStorageBuffer = 0;

    public ShaderBuilder(GPUDevice device, ShaderCross.ShaderStage stage, string src)
    {
        _device = device;
        this.stage = stage;
        this.src = src;
    }

    public Shader Build()
    {
        return new Shader(this);
    }

    public nint Compile()
    {
        ShaderCross.HLSLInfo hlsl = default;
        hlsl.IncludeDir = nint.Zero;
        hlsl.Defines = nint.Zero;
        hlsl.ShaderStage = stage;
        hlsl.ManagedSource = src;
        hlsl.ManagedEntrypoint = entryPoint;
        hlsl.ManagedIncludeDir = null;
        nint spirv = ShaderCross.CompileSPIRVFromHLSL(ref hlsl, out var size);
        if (spirv == 0)
        {
            Logger.Fatal("Renderer", $"Failed to compile HLSL shader to SPIR-V: {SDL.GetError()}");
        }

        ShaderCross.SPIRVInfo spirvInfo = default;
        spirvInfo.ByteCode = spirv;
        spirvInfo.ByteCodeSize = size;
        spirvInfo.ManagedEntrypoint = "main";
        spirvInfo.ShaderStage = stage;
        spirvInfo.Props = 0;

        nint metadataPtr = ShaderCross.ReflectGraphicsSPIRV(spirv, size, 0);
        if (metadataPtr == 0)
        {
            Logger.Fatal("Renderer", $"Failed to reflect SPIR-V shader: {SDL.GetError()}");
        }

        ShaderCross.GraphicsShaderMetadata metadata =
            Marshal.PtrToStructure<ShaderCross.GraphicsShaderMetadata>(metadataPtr);

        ShaderCross.GraphicsShaderResourceInfo info = metadata.ResourceInfo;
        nint shader = ShaderCross.CompileGraphicsShaderFromSPIRV(
            _device.device,
            ref spirvInfo,
            ref info,
            0
        );
        if (shader == 0)
        {
            Logger.Fatal("Renderer", $"Failed to compile HLSL shader to SPIR-V: {SDL.GetError()}");
        }
        SDL.Free(metadataPtr);
        SDL.Free(spirv);
        return shader;
    }
}
