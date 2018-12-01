#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float3 CameraPosition; // in world space
float4x4 World; // World Matrix
float4x4 View; // View Matrix
float4x4 Projection; // Projection Matrix

struct VertexShaderInput
{
    float4 Position : SV_Position0;
   // float3 Normal : NORMAL0;
    float2 UV : TEXCOORD0;
    //float4 Color : COLOR0;
    //float2 UV: TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	//float4 Color : COLOR0;
    float4 WorldPosition : TEXCOORD1;
    //float3 WorldNormal : TEXCOORD2;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
    output.WorldPosition = mul(input.Position, World);
    float4 viewPosition = mul(output.WorldPosition, View);
    output.Position = mul(viewPosition, Projection);

    //output.Color = input.Color; //olor : COLOR0;
	// Send normal in world space
   /// output.WorldNormal = mul(input.Normal, World);
	

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    //bool edge = dot(CameraPosition, input.WorldNormal) == 0;
    //float4 color = (1, 1, 1, ((int) edge));
    float dist = 1 / (distance(CameraPosition, input.WorldPosition)/15);

    return float4(1, 0, 0, dist*dist); //input.Color;

}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};