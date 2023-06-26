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
    float4 _CascadeData[MAX_CASCADE_COUNT];
    float4x4 _DirectionalShadowMatrices[MAX_SHADOWED_DIRECTIONAL_LIGHT_COUNT * MAX_CASCADE_COUNT];
    // float _ShadowDistance;
    float4 _ShadowDistanceFade;
CBUFFER_END

//这个是场景中原本平行光的自带属性，strength越小，影子越淡
struct DirectionalShadowData {
    float strength;
    int tileIndex;      //settings.directional.cascadeCount * ShadowedDirectionalLightCount
    float normalBias;
};

struct ShadowData {
    int cascadeIndex;
    float cascadeBlend;
    float strength;     //表示计算过程中 随着距离等因素造成的衰减, 0表示没有影子
};

//todo 这个函数被两次调用，传入的参数有点混乱
float _FadedShadowStrength (float distance, float scale, float fade) {
    return saturate((1.0 - distance * scale) * fade);
}

ShadowData GetShadowData (Surface surfaceWS) {
    ShadowData data;
    data.strength = _FadedShadowStrength(
        surfaceWS.depth, _ShadowDistanceFade.x, _ShadowDistanceFade.y
    );
    int i;
    for (i = 0; i < _CascadeCount; i++) {
        float4 sphere = _CascadeCullingSpheres[i];
        float distanceSqr = DistanceSquared(surfaceWS.position, sphere.xyz);
        if (distanceSqr < sphere.w) {
            if (i == _CascadeCount - 1) {
                // data.strength = 0;
                data.strength *= _FadedShadowStrength(
                    distanceSqr, _CascadeData[i].x, _ShadowDistanceFade.z
                );
            }
            break;
        }
    }
    if (i == _CascadeCount) {
        data.strength = 0.0;
    }
    // data.strength = 0;
    data.cascadeIndex = i;
    return data;
}

float _SampleDirectionalShadowAtlas (float3 positionSTS) {
    //回[0,1]的值，也就是上一步SampleCmp函数得到的结果，该值反映了阴影的衰减度ShadowAttenuation,
    //0意味着片元完全在阴影中，1意味着片元完全不在阴影中，而中间值意味着片元有部分在阴影中
    return SAMPLE_TEXTURE2D_SHADOW(
        _DirectionalShadowAtlas, SHADOW_SAMPLER, positionSTS
    );
}

//被light.hlsl调用, 返回的值如果是1，说明不在阴影内
float GetDirectionalShadowAttenuation (
    DirectionalShadowData directional, ShadowData global, Surface surfaceWS
) {
    if (directional.strength <= 0.0) {
        return 1.0;
    }
    float3 normalBias = surfaceWS.normal * _CascadeData[global.cascadeIndex].y;
    float3 positionSTS = mul(
        _DirectionalShadowMatrices[directional.tileIndex],
        float4(surfaceWS.position + normalBias, 1.0)
    ).xyz;
    float shadow = _SampleDirectionalShadowAtlas(positionSTS);
    // return shadow;
    // return 0;
    return lerp(1.0, shadow, directional.strength); 
}

#endif