#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
struct Attributes
{
    uint vertex     : POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct VertexData {
	float3 position : POSITION;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD0;
};

struct Varyings
{
#if defined(_ALPHATEST_ON)
    float2 uv       : TEXCOORD0;
#endif
    float4 positionCS   : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
        UNITY_VERTEX_OUTPUT_STEREO
};


VertexData UnpackVertex(Attributes a, uint vertexId)
{
	// LOD
	int lod = a.vertex & 3;
	int lodIncrement = pow(4, lod);
	int colWidthInVertices = 32 / lodIncrement;
	// POSITION
	int x = (vertexId / (colWidthInVertices + 1)) * lodIncrement;
	int z = (vertexId % (colWidthInVertices + 1)) * lodIncrement;
	int y = (a.vertex >> 2) & 1023;
	VertexData vertexData = (VertexData)0;
	vertexData.position = float3(x, (y / 10.0f), z);

	// Normals
	int nx = (a.vertex >> 12) & 15;
	int ny = (a.vertex >> 16) & 15;
	int nz = (a.vertex >> 20) & 15;
	// Normal signs of x & z.
	int sx = (a.vertex >> 24) & 1;
	int sz = (a.vertex >> 25) & 1;
	float unx = ((nx / 10.0f) * (sx)) + ((nx / 10.0f) * (sx - 1));
	float unz = ((nz / 10.0f) * (sz)) + ((nz / 10.0f) * (sz - 1));
	vertexData.normal = float3(unx, (ny / 10.0f), unz);

	// UV
	float u = vertexId % 2;
	float v = (vertexId / (colWidthInVertices + 1)) % 2;
	vertexData.uv = float2(u, v);

	return vertexData;
}
Varyings DepthOnlyVertex(Attributes input, uint vertexID : SV_VertexID)
{
	VertexData vertexData = UnpackVertex(input, vertexID);
    Varyings output = (Varyings)0;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

#if defined(_ALPHATEST_ON)
    output.uv = TRANSFORM_TEX(vertexData.uv, _BaseMap);
#endif
    output.positionCS = TransformObjectToHClip(vertexData.position.xyz);
    return output;
}


half DepthOnlyFragment(Varyings input) : SV_TARGET
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    #if defined(_ALPHATEST_ON)
        Alpha(SampleAlbedoAlpha(input.uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)).a, _BaseColor, _Cutoff);
    #endif

    #if defined(LOD_FADE_CROSSFADE)
        LODFadeCrossFade(input.positionCS);
    #endif

    return input.positionCS.z;
}
