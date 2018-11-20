#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
float offset;
float height;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 position = (input.TextureCoordinates.x > 0.5)
    ? float2(input.TextureCoordinates.x + offset, 0.2 * abs(input.TextureCoordinates.x - 0.5) / abs(input.TextureCoordinates.y - 0.5))
 : float2(input.TextureCoordinates.x - offset, 0.2 * abs(input.TextureCoordinates.x - 0.5) / abs(input.TextureCoordinates.y - 0.5));
    position.x %= 1;
    position.x = abs(position.x);
    if(position.y < 0 || position.y > 1) position.y = position.x;
	return tex2D(SpriteTextureSampler, position) * input.Color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};