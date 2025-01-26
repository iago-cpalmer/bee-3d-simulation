#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

void MainLight_float(float3 WorldPos, out float3 Direction, out float3 Color, out float DistanceAtten, out float ShadowAtten)
{
#if SHADERGRAPH_PREVIEW
    Direction = float3(0.5, 0.5, 0);
    Color = 1;
    DistanceAtten = 1;
    ShadowAtten = 1;
#else
#if SHADOWS_SCREEN
    float4 clipPos = TransformWorldToHClip(WorldPos);
    float4 shadowCoord = ComputeScreenPos(clipPos);
#else
    float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif
    Light mainLight = GetMainLight(shadowCoord);
    Direction = mainLight.direction;
    Color = mainLight.color;
    DistanceAtten = mainLight.distanceAttenuation;
    ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
    float shadowStrength = GetMainLightShadowStrength();
    ShadowAtten = SampleShadowmap(shadowCoord, TEXTURE2D_ARGS(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture), shadowSamplingData, shadowStrength, false);
#endif
}

void AdditionalLights_float(float3 WorldPosition, float3 WorldNormal, out float3 Diffuse)
{
    float3 diffuseColor = 0;

    #ifndef SHADERGRAPH_PREVIEW
        int pixelLightCount = GetAdditionalLightsCount();
        for (int i = 0; i < pixelLightCount; ++i)
        {
            Light light = GetAdditionalLight(i, WorldPosition, half4(1,1,1,1));
            half3 attenuatedLightColor = light.color * (min(light.distanceAttenuation,0.1f) * light.shadowAttenuation);
            diffuseColor += LightingLambert(attenuatedLightColor, light.direction, WorldNormal);
        }
    #endif

        Diffuse = diffuseColor;
}


float GetLightIntensity(float3 color) {
    return max(color.r, max(color.g, color.b));
}

/*
void AddAdditionalLights_float(float Smoothness, float3 WorldPosition, float3 WorldNormal, float3 WorldView,
    float MainDiffuse, float MainSpecular, float3 MainColor,
    out float Diffuse, out float Specular, out float3 Color) {

    float3 mainIntensity = GetLightIntensity(MainColor);
    Diffuse = MainDiffuse * mainIntensity;
    Specular = MainSpecular * mainIntensity;
    Color = MainColor;
#ifndef SHADERGRAPH_PREVIEW
    float maxDiffuse = Diffuse;

    int pixelLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < pixelLightCount; ++i) {
        Light light = GetAdditionalLight(i, WorldPosition);
        half NdotL = saturate(dot(WorldNormal, light.direction));
        half atten = light.distanceAttenuation * light.shadowAttenuation * GetLightIntensity(Color);
        half thisDiffuse = atten * NdotL;
        half thisSpecular = LightingSpecular(thisDiffuse, light.direction, WorldNormal, WorldView, 1, Smoothness);
        Diffuse += thisDiffuse;
        Specular += thisSpecular;

        if (thisDiffuse > maxDiffuse) {
            maxDiffuse = thisDiffuse;
            Color = light.color;
        }
    }

#endif

    half total = Diffuse + Specular;
    // If no light touches this pixel, set the color to the main light's color
    Color = total <= 0 ? MainColor : Color / total;
}
*/
#endif