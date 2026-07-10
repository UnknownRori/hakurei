namespace Hakurei.Engine.Renderer;

using SDL3;
using System.Runtime.InteropServices;

public class GraphicsPipelineBuilder<T>
{
    public List<SDL.GPUVertexAttribute> vertexAttributes = new List<SDL.GPUVertexAttribute>();
    public SDL.GPUPrimitiveType primitiveType = SDL.GPUPrimitiveType.TriangleList;
    public Shader frag;
    public Shader vert;

    public GraphicsPipelineBuilder(Shader frag, Shader vert)
    {
        this.frag = frag;
        this.vert = vert;
    }

    public GraphicsPipeline Build(Window win, GPUDevice device)
    {
        var colorTargetDescPtr = GetTargetDescription(device, win);
        GetVertexDescription(out nint vertexBufDescPtr, out nint vertexAttrPtr, out SDL.GPUVertexAttribute[] vertexAttr);
        var pipelineInfo = GetPipelineDescription(colorTargetDescPtr, vertexBufDescPtr, vertexAttrPtr, vertexAttr);
        var pipelineId = CreatePipeline(device, pipelineInfo);

        Marshal.FreeHGlobal(colorTargetDescPtr);
        Marshal.FreeHGlobal(vertexAttrPtr);
        Marshal.FreeHGlobal(vertexBufDescPtr);

        vert.Dispose();
        frag.Dispose();

        return new GraphicsPipeline(device, pipelineId);
    }

    private nint GetTargetDescription(GPUDevice device, Window win)
    {
        var colorTargetDesc = new SDL.GPUColorTargetDescription
        {
            Format = SDL.GetGPUSwapchainTextureFormat(device.device, win.window),
            BlendState = new SDL.GPUColorTargetBlendState
            {
                EnableBlend = true,
                SrcColorBlendFactor = SDL.GPUBlendFactor.SrcAlpha,
                DstColorBlendFactor = SDL.GPUBlendFactor.OneMinusSrcAlpha,
                ColorBlendOp = SDL.GPUBlendOp.Add,
                SrcAlphaBlendFactor = SDL.GPUBlendFactor.One,
                DstAlphaBlendFactor = SDL.GPUBlendFactor.OneMinusSrcAlpha,
                AlphaBlendOp = SDL.GPUBlendOp.Add,
            }
        };
        var colorTargetDescPtr = SDL.StructureToPointer<SDL.GPUColorTargetDescription>(colorTargetDesc);
        if (colorTargetDescPtr == 0)
        {
            throw new Exception("[FATAL] GraphicsPipeline: Failed to create color target description");
        }
        return colorTargetDescPtr;
    }

    private void GetVertexDescription(out nint vertexBufDescPtr, out nint vertexAttrPtr, out SDL.GPUVertexAttribute[] vertexAttr)
    {
        vertexAttr = vertexAttributes.ToArray();
        int attrStride = Marshal.SizeOf<SDL.GPUVertexAttribute>();
        vertexAttrPtr = Marshal.AllocHGlobal(attrStride * vertexAttr.Length);
        for (int i = 0; i < vertexAttr.Length; i++)
            Marshal.StructureToPtr(vertexAttributes[i], vertexAttrPtr + i * attrStride, false);
        var vertexBufferDesc = new SDL.GPUVertexBufferDescription
        {
            Slot = 0,
            Pitch = (uint)Marshal.SizeOf<T>(),
            InputRate = SDL.GPUVertexInputRate.Vertex
        };
        vertexBufDescPtr = SDL.StructureToPointer<SDL.GPUVertexBufferDescription>(vertexBufferDesc);
    }

    private SDL.GPUGraphicsPipelineCreateInfo GetPipelineDescription(nint colorTargetDescPtr, nint vertexBufDescPtr, nint vertexAttrPtr, SDL.GPUVertexAttribute[] vertexAttr)
    {
        SDL.GPUGraphicsPipelineCreateInfo pipelineInfo = new SDL.GPUGraphicsPipelineCreateInfo();
        pipelineInfo.FragmentShader = frag.shader;
        pipelineInfo.VertexShader = vert.shader;
        pipelineInfo.PrimitiveType = primitiveType;
        pipelineInfo.MultisampleState = new SDL.GPUMultisampleState();
        pipelineInfo.VertexInputState = new SDL.GPUVertexInputState
        {
            VertexAttributes = vertexAttrPtr,
            NumVertexAttributes = (uint)vertexAttr.Length,
            VertexBufferDescriptions = vertexBufDescPtr,
            NumVertexBuffers = 1
        };
        pipelineInfo.RasterizerState = new SDL.GPURasterizerState
        {
            FillMode = SDL.GPUFillMode.Fill,
            CullMode = SDL.GPUCullMode.None,
            FrontFace = SDL.GPUFrontFace.CounterClockwise
        };
        pipelineInfo.TargetInfo = new SDL.GPUGraphicsPipelineTargetInfo
        {
            ColorTargetDescriptions = colorTargetDescPtr,
            NumColorTargets = 1,
            HasDepthStencilTarget = false,
        };

        return pipelineInfo;
    }

    private nint CreatePipeline(GPUDevice device, SDL.GPUGraphicsPipelineCreateInfo pipelineInfo)
    {
        nint pipelineId = SDL.CreateGPUGraphicsPipeline(device.device, pipelineInfo);
        if (pipelineId == 0)
        {
            Logger.Fatal("Renderer", $"Failed to create GraphicsPipeline: {SDL.GetError()}");
        }
        return pipelineId;
    }
}

