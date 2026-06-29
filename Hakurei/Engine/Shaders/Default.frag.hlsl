Texture2D tex : register(t0, space2);
SamplerState samp : register(s0, space2);

struct Input
{
    float4 position : SV_Position;
    float4 color : COLOR0;
    float2 uv    : TEXCOORD0;
};

float4 main(Input input) : SV_Target0
{
    return tex.Sample(samp, input.uv) * input.color;
}