// TODO : Add Uniform buffer for color tint

struct Input
{
    float3 position : POSITION;
    float2 uv       : TEXCOORD0;
};

struct Output
{
    float4 position : SV_Position;
    float4 color    : COLOR0;
    float2 uv       : TEXCOORD0;
};

Output main(Input input)
{
    Output output;
    output.position = float4(input.position, 1.0);
    output.color = float4(1.0, 1.0, 1.0, 1.0);
    output.uv = input.uv;
    return output;
}