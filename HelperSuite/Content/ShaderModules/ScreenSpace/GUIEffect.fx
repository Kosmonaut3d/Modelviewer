
Texture2D Tex;
SamplerState PointSampler
{
    Texture = <Tex>; 
	MagFilter = POINT;
	MinFilter = POINT;
	Mipfilter = POINT;
	AddressU = CLAMP;
	AddressV = CLAMP;
};

struct VertexShaderInput
{
	float3 Position : POSITION0;
	float4 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float4 TexCoord : TEXCOORD0;
}; 

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	output.Position = float4(input.Position, 1);
	output.TexCoord = input.TexCoord;
	return output;
}

float4 FlatPS(VertexShaderOutput input) : SV_TARGET0
{
	return Tex.Sample(PointSampler, input.TexCoord);
}


technique Texture
{
	pass Texture
	{
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 FlatPS();
	}
}