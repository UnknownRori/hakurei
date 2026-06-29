using SDL3;

namespace Hakurei.Engine.Renderer;

public class DefaultShader
{
    public readonly Shader vert;
    public readonly Shader frag;

    public DefaultShader(GPUDevice device)
    {
        LoadShader(device, out vert, out frag);
    }

    public static void LoadShader(GPUDevice device, out Shader vert, out Shader frag)
    {
        string vertSrc = File.ReadAllText("Engine/Shaders/Default.vert.hlsl");
        ShaderBuilder vertBuilder = new ShaderBuilder(device, ShaderCross.ShaderStage.Vertex, vertSrc);
        vert = vertBuilder.Build();

        string fragSrc = File.ReadAllText("Engine/Shaders/Default.frag.hlsl");
        ShaderBuilder fragBuilder = new ShaderBuilder(device, ShaderCross.ShaderStage.Fragment, fragSrc);
        frag = fragBuilder.Build();
    }
}
