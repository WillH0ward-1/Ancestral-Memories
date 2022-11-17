#ifndef MB_CARDINAL
#define MB_CARDINAL

//IDE Only Defines & Methods
#if __INTELLISENSE__
#define _MAIN_LIGHT_SHADOWS
#define _ADDITIONAL_LIGHTS

#include "UnityCG.cginc"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/GlobalIllumination.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RealtimeLights.hlsl"
#endif

struct CardinalData
{
    //Surface
    float3 albedo;
    float smoothness;

    //coordinates
    float3 position;
    float3 normal;
    float3 viewDirection;
    float4 shadowCoord;
};

struct AmbientLightVolume
{
    float4x4 space;
    float3 size;
    float3 padding;

    float4 color;
};
uniform StructuredBuffer<AmbientLightVolume> MB_AmbientLightVolumeBuffer;
uniform uint MB_AmbientLightVolumeCount;

float InverseLerp(float a, float b, float value)
{
    return (value - a) / (b - a);
}

float CalculateAmbientFactor(float delta, float size1, float size2)
{
    delta = abs(delta);

    float factor = InverseLerp(size1, size2, delta);
    factor = clamp(factor, 0, 1);
    factor = 1 - factor;
    return factor;
}

float4 SampleAmbientLightVolume(float3 position, AmbientLightVolume volume)
{
    float4 color = volume.color;

    //Calculate Local Position
    float3 delta = mul(volume.space, float4(position, 1)).xyz;

    float3 size1 = volume.size / 2;
    float3 size2 = (volume.size + volume.padding) / 2;

    float xFactor = CalculateAmbientFactor(delta.x, size1.x, size2.x);
    float yFactor = CalculateAmbientFactor(delta.y, size1.y, size2.y);
    float zFactor = CalculateAmbientFactor(delta.z, size1.z, size2.z);
    
    float factor = min(min(xFactor, yFactor), min(xFactor, zFactor));

    color.a *= factor;
    
    return color;
}
float3 SampleAmbientLight(float3 position, float3 normal)
{
#ifdef SHADERGRAPH_PREVIEW
    return float3(0.1, 0.1, 0.1); // Default ambient colour for previews
#else
    float4 target = float4(SampleSH(normal), 1);

    for (uint index = 0; index < MB_AmbientLightVolumeCount; index++)
    {
        float4 value = SampleAmbientLightVolume(position, MB_AmbientLightVolumeBuffer[index]);
        target = lerp(target, value, value.a);
    }
    
    return target.rgb;
#endif
}

float GetSmoothnessPower(float raw)
{
    return exp2(10 * raw + 1);
}

#ifndef SHADERGRAPH_PREVIEW
float3 Shade(CardinalData data, Light light)
{
    float3 radiance = light.color * light.distanceAttenuation * light.shadowAttenuation;

    float3 diffuse = saturate(dot(data.normal, light.direction));

    float3 specularDot = saturate(dot(data.normal, normalize(light.direction + data.viewDirection)));
    float3 specular = pow(specularDot, GetSmoothnessPower(data.smoothness)) * diffuse;

    float3 color = data.albedo * radiance * (diffuse + specular);

    return color;
}

float3 Shade(CardinalData data)
{
    float3 color = 0;

#ifdef _MAIN_LIGHT_SHADOWS
    Light main = GetMainLight(data.shadowCoord, data.position, 1);
    color += Shade(data, main);
#endif

#ifdef _ADDITIONAL_LIGHTS
    uint additionalLightsCount = GetAdditionalLightsCount();
    for (uint index = 0; index < additionalLightsCount; index++)
    {
        Light light = GetAdditionalLight(index, data.position, 1);
        color += Shade(data, light);
    }
#endif

    return color;
}
#endif

float4 GetShadowCoords(float3 position)
{
#ifdef SHADERGRAPH_PREVIEW
    return 0;
#else

#if SHADOWS_SCREEN
    return ComputeScreenPos(TransformWorldToHClip(position));
#else
    return TransformWorldToShadowCoord(position);
#endif

#endif
}

void Calculate_float(float3 albedo, float smoothness, float3 position, float3 normal, float3 viewDirection, out float3 color)
{
    CardinalData data;

    data.albedo = albedo;
    data.smoothness = smoothness;
    data.position = position;
    data.normal = normal;
    data.viewDirection = viewDirection;
    data.shadowCoord = GetShadowCoords(position);

#ifdef SHADERGRAPH_PREVIEW
    color = albedo;
#else
    color = Shade(data);
    color += albedo * SampleAmbientLight(position, normal);
#endif
}
#endif