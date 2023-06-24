#ifndef CUSTOM_SHADOWS_INCLUDED
#define CUSTOM_SHADOWS_INCLUDED

#define MAX_SHADOWED_DIRECTIONAL_LIGHT_COUNT 4
#define MAX_CASCADE_COUNT 4

TEXTURE2D(_DirectionalShadowAtlas);
#define SHADOW_SAMPLER sampler_linear_clamp_compare
SAMPLER_CMP(SHADOW_SAMPLER);

CBUFFER_START(_CustomShadows)
    int _CascadeCount;
    float4 _CascadeCullingSpheres[MAX_CASCADE_COUNT];
    float4x4 _DirectionalShadowMatrices[MAX_SHADOWED_DIRECTIONAL_LIGHT_COUNT * MAX_CASCADE_COUNT];
    // float _ShadowDistance;
    float4 _ShadowDistanceFade;
CBUFFER_END

//这个是场景中原本平行光的自带属性，strength越小，影子越淡
struct DirectionalShadowData {
    float strength;
    int tileIndex;
    float normalBias;
};

struct ShadowData {
    int cascadeIndex;
    float cascadeBlend;
    float strength;     //表示计算过程中 随着距离等因素造成的衰减
};

float FadedShadowStrength (float distance, float scale, float fade) {
    return saturate((1.0 - distance * scale) * fade);
}

ShadowData GetShadowData (Surface surfaceWS) {
    ShadowData data;
    data.strength = FadedShadowStrength(
        surfaceWS.depth, _ShadowDistanceFade.x, _ShadowDistanceFade.y
    );
    int i;
    for (i = 0; i < _CascadeCount; i++) {
        float4 sphere = _CascadeCullingSpheres[i];
        float distanceSqr = DistanceSquared(surfaceWS.position, sphere.xyz);
        if (distanceSqr < sphere.w) {
            break;
        }
    }
    if (i == _CascadeCount) {
        data.strength = 0.0;
    }

    data.cascadeIndex = i;
    return data;
}

float _SampleDirectionalShadowAtlas (float3 positionSTS) {
    return SAMPLE_TEXTURE2D_SHADOW(
        _DirectionalShadowAtlas, SHADOW_SAMPLER, positionSTS
    );
}

float GetDirectionalShadowAttenuation (DirectionalShadowData data, Surface surfaceWS) {
    if (data.strength <= 0.0) {
        return 1.0;
    }
    float3 positionSTS = mul(
        _DirectionalShadowMatrices[data.tileIndex],
        float4(surfaceWS.position, 1.0)
    ).xyz;
    float shadow = _SampleDirectionalShadowAtlas(positionSTS); //理论上是0和1两个值，0代表是在阴影中
    return lerp(1.0, shadow, data.strength);
}

#endif