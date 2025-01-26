#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Assets/Shaders/CustomLighting.hlsl"

struct Attributes {
	uint vertex : POSITION;
};

struct VertexData {
	float3 position : POSITION;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD0;
};

struct Interpolators {
	float4 positionCS : SV_POSITION;
	float3 positionWS : WS_POSITION;
	float3 normalWS : NORMAL;
	float2 uv : TEXCOORD0;
	half4 fogFactor : TEXCOORD5;
};

TEXTURE2D(_AlbedoTexture); SAMPLER(sampler_AlbedoTexture);
CBUFFER_START(UnityPerMaterial)
float4 _AlbedoTexture_ST;
CBUFFER_END

VertexData UnpackVertex(Attributes a, uint vertexId)
{
	// LOD
	int lod = a.vertex & 3;
	int lodIncrement = pow(4, lod);
	int colWidthInVertices = 32 / lodIncrement;
	// POSITION
	int x = (vertexId / (colWidthInVertices + 1)) * lodIncrement;
	int z = (vertexId % (colWidthInVertices + 1)) * lodIncrement;
	int y = (a.vertex >>2) & 1023;
	VertexData vertexData = (VertexData)0;
	vertexData.position = float3(x, (y / 10.0f), z);

	// Normals
	int nx = (a.vertex >> 12) & 15;
	int ny = (a.vertex >> 16) & 15;
	int nz = (a.vertex >> 20) & 15;
	// Normal signs of x & z.
	int sx = (a.vertex >> 24) & 1;
	int sz = (a.vertex >> 25) & 1;
	float unx = ((nx / 10.0f) * (sx)) + ((nx / 10.0f) * (sx-1));
	float unz = ((nz / 10.0f) * (sz)) + ((nz / 10.0f) * (sz-1));
	vertexData.normal = float3(unx, (ny / 10.0f), unz);

	// UV
	float u = vertexId % 2;
	float v = (vertexId / (colWidthInVertices + 1))%2;
	vertexData.uv = float2(u,v);

	return vertexData;
}

Interpolators Vertex(Attributes input, uint vertexID : SV_VertexID) {

	VertexData vertexData = UnpackVertex(input, vertexID);

	VertexPositionInputs posInputs = GetVertexPositionInputs(vertexData.position);
	Interpolators output;

	output.positionCS = posInputs.positionCS;
	output.positionWS = posInputs.positionWS;
	output.uv = TRANSFORM_TEX(vertexData.uv, _AlbedoTexture);
	output.normalWS = GetVertexNormalInputs(vertexData.normal).normalWS;

	half4 fogFactor = ComputeFogFactor(posInputs.positionCS.z);
	output.fogFactor = fogFactor;

	return output;
}


float4 Fragment(Interpolators input) : SV_Target{
	float4 color = float4(SAMPLE_TEXTURE2D(_AlbedoTexture, sampler_AlbedoTexture, input.uv).rgb, 1);
	
	// Main light
	
	float3 direction, lightColor;
	float distanceAtten, shadowAtten;

	MainLight_float(input.positionWS, direction, lightColor, distanceAtten, shadowAtten);
	float diff = max(dot(input.normalWS, direction), 0);
	color.rgb *= diff * lightColor * shadowAtten;
	float3 fogCoord = InitializeInputDataFog(float4(input.positionWS, 1.0), input.fogFactor);
	color.rgb = MixFog(color.rgb, fogCoord);
	//depth = input.positionCS.z;
	return color;
}