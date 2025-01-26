#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#if defined(LOD_FADE_CROSSFADE)
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
#endif

#if defined(_DETAIL_MULX2) || defined(_DETAIL_SCALED)
#define _DETAIL
#endif

#if defined(_PARALLAXMAP)
#define REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR
#endif

#if (defined(_NORMALMAP) || (defined(_PARALLAXMAP) && !defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR))) || defined(_DETAIL)
#define REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR
#endif

#if defined(_ALPHATEST_ON) || defined(_PARALLAXMAP) || defined(_NORMALMAP) || defined(_DETAIL)
#define REQUIRES_UV_INTERPOLATOR
#endif

struct Attributes
{
    uint vertex : POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct VertexData {
    float3 position : POSITION;
    float3 normal : NORMAL;
    float2 uv : TEXCOORD0;
};

struct Varyings
{
    float4 positionCS  : SV_POSITION;
#if defined(REQUIRES_UV_INTERPOLATOR)
    float2 uv          : TEXCOORD1;
#endif
    half3 normalWS     : TEXCOORD2;

#if defined(REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR)
    half4 tangentWS    : TEXCOORD4;    // xyz: tangent, w: sign
#endif

    half3 viewDirWS    : TEXCOORD5;

#if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
    half3 viewDirTS    : TEXCOORD8;
#endif

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

Varyings DepthNormalsVertex(Attributes input, uint vertexID : SV_VertexID)
{
    VertexData vertexData = UnpackVertex(input, vertexID);
    Varyings output = (Varyings)0;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

#if defined(REQUIRES_UV_INTERPOLATOR)
    output.uv = TRANSFORM_TEX(vertexData.uv, _BaseMap);
#endif
    output.positionCS = TransformObjectToHClip(vertexData.position.xyz);

    VertexPositionInputs vertexInput = GetVertexPositionInputs(vertexData.position.xyz);
    output.normalWS = GetVertexNormalInputs(vertexData.normal).normalWS;

    return output;
}

void DepthNormalsFragment(
    Varyings input
    , out half4 outNormalWS : SV_Target0
#ifdef _WRITE_RENDERING_LAYERS
    , out float4 outRenderingLayers : SV_Target1
#endif
)
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

#if defined(_ALPHATEST_ON)
    Alpha(SampleAlbedoAlpha(input.uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)).a, _BaseColor, _Cutoff);
#endif

#if defined(LOD_FADE_CROSSFADE)
    LODFadeCrossFade(input.positionCS);
#endif

#if defined(_GBUFFER_NORMALS_OCT)
    float3 normalWS = normalize(input.normalWS);
    float2 octNormalWS = PackNormalOctQuadEncode(normalWS);           // values between [-1, +1], must use fp32 on some platforms
    float2 remappedOctNormalWS = saturate(octNormalWS * 0.5 + 0.5);   // values between [ 0,  1]
    half3 packedNormalWS = PackFloat2To888(remappedOctNormalWS);      // values between [ 0,  1]
    outNormalWS = half4(packedNormalWS, 0.0);
#else
#if defined(_PARALLAXMAP)
#if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
    half3 viewDirTS = input.viewDirTS;
#else
    half3 viewDirTS = GetViewDirectionTangentSpace(input.tangentWS, input.normalWS, input.viewDirWS);
#endif
    ApplyPerPixelDisplacement(viewDirTS, input.uv);
#endif

#if defined(_NORMALMAP) || defined(_DETAIL)
    float sgn = input.tangentWS.w;      // should be either +1 or -1
    float3 bitangent = sgn * cross(input.normalWS.xyz, input.tangentWS.xyz);
    float3 normalTS = SampleNormal(input.uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), _BumpScale);

#if defined(_DETAIL)
    half detailMask = SAMPLE_TEXTURE2D(_DetailMask, sampler_DetailMask, input.uv).a;
    float2 detailUv = input.uv * _DetailAlbedoMap_ST.xy + _DetailAlbedoMap_ST.zw;
    normalTS = ApplyDetailNormal(detailUv, normalTS, detailMask);
#endif

    float3 normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS.xyz));
#else
    float3 normalWS = input.normalWS;
#endif

    outNormalWS = half4(NormalizeNormalPerPixel(normalWS), 0.0);
#endif

#ifdef _WRITE_RENDERING_LAYERS
    uint renderingLayers = GetMeshRenderingLayer();
    outRenderingLayers = float4(EncodeMeshRenderingLayer(renderingLayers), 0, 0, 0);
#endif
}