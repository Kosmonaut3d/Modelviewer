/*
Copyright 2017 by kosmonautgames

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

// Basic Skybox shader

////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  Variables

float4x4 WorldViewProj;
float3x3 World;

TextureCube SkyboxTexture;
SamplerState CubeMapSampler
{
	texture = <ReflectionCubeMap>;
	AddressU = CLAMP;
	AddressV = CLAMP;
	MagFilter = LINEAR;
	MinFilter = LINEAR;
	Mipfilter = LINEAR;
};

////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  Structs

struct DrawBasic_VSIn
{
	float4 Position : POSITION0;
	float3 Normal   : NORMAL0;
	float2 TexCoord : TEXCOORD0;
};

////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  Functions

DrawBasic_VSIn DrawSkybox_VertexShader(DrawBasic_VSIn input)
{
	DrawBasic_VSIn Output;
	//input.Position.z *= input.Position.z+0.5f;
	Output.Position = mul(input.Position, WorldViewProj);
	Output.Normal = mul(input.Normal, World);
	Output.TexCoord = input.TexCoord;
	return Output;
}

float4 DrawSkybox_PixelShader(DrawBasic_VSIn input) : COLOR
{
	float3 normal = normalize(input.Normal);
	return SkyboxTexture.SampleLevel(CubeMapSampler, normal.xzy,2);
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  Techniques

technique DrawSkybox
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 DrawSkybox_VertexShader();
		PixelShader = compile ps_5_0 DrawSkybox_PixelShader();
	}
}