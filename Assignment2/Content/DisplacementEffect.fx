// Parameters that should be set from the program
float4x4 World; // World Matrix
float4x4 View; // View Matrix
float4x4 Projection; // Projection Matrix
float3 CameraPosition; // in world space
texture DiffuseTexture;

// Extra effect textures
texture DispTex;
texture RampTex;
float3 ChannelFactor;
float Displacement;


sampler DiffuseSampler = sampler_state
{
	Texture = <DiffuseTexture>;
	AddressU = Wrap;
	AddressV = Wrap;
};

sampler DispSampler = sampler_state
{
    Texture = <DispTex>;
    AddressU = Wrap;
    AddressV = Wrap;
};

sampler RampSampler = sampler_state
{
    Texture = <RampTex>;
    AddressU = Wrap;
    AddressV = Wrap;
};

// We create structs to help us manage the inputs/outputs
// to vertex and pixel shaders
struct VertexInput
{
    float4 Position : POSITION0; // Here, POSITION0 and NORMAL0
	float3 Normal : NORMAL0; // are called mnemonics
	float2 UV: TEXCOORD0;
};

struct StandardVertexOutput
{
	float4 Position : POSITION0;
	float2 UV: TEXCOORD0;
	float4 WorldPosition : TEXCOORD1;
	float3 WorldNormal : TEXCOORD2;
};

// Vertex shader that caculates offset from a noise texture
StandardVertexOutput DisplacementVertex(VertexInput input)
{
	StandardVertexOutput output;
    
    // Sample the displacement texture and offset position in direction of normals
    float3 dcolor = tex2Dlod(DispSampler, float4(input.UV.xy, 0, 0));
    float d = (dcolor.r * ChannelFactor.r + dcolor.g * ChannelFactor.g + dcolor.b * ChannelFactor.b);
    input.Position += float4(input.Normal * d * Displacement * Displacement, 1); // This line may be wrong

	// Do the transformations as before
	// Save the world position for use in the pixel shader
	output.WorldPosition = mul(input.Position, World);
	float4 viewPosition = mul(output.WorldPosition, View);
	output.Position = mul(viewPosition, Projection);

	// as well as the normal in world space
	output.WorldNormal = mul(input.Normal, World);
	output.UV = input.UV;
	return output;
}

// The pixel shader that calculates color off a ramp texture
float4 RampEffectPixel(StandardVertexOutput input) : COLOR0
{
    // Sample the main texture
    float4 col = tex2D(DiffuseSampler, input.UV);

    // Find a value based on displacement
    float d = (col.r * ChannelFactor.r + col.g * ChannelFactor.g + col.b * ChannelFactor.b) * (Displacement); //* (_Range.y-_Range.x) + _Range.x;
 
    d = (col.r * ChannelFactor.r + col.g * ChannelFactor.g + col.b * ChannelFactor.b);

    // Sample and return color value from the ramp texture
    return tex2D(RampSampler, float2(d * (1 - Displacement), 0.5));
}

technique RampDisplacement
{
	pass Pass1
	{
        VertexShader = compile vs_4_0 DisplacementVertex();
        PixelShader = compile ps_4_0 RampEffectPixel();
    }
}