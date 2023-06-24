#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

float3 _IncomingLight (Surface surface, Light light) {
    //saturate(v): 将v夹取到 [0,1]区间.
    return saturate(dot(surface.normal, light.direction) * light.attenuation) * light.color;
}

float3 _GetLighting (Surface surface, BRDF brdf, Light light) {
    return _IncomingLight(surface, light) * DirectBRDF(surface, brdf, light);
}

float3 GetLighting (Surface surface, BRDF brdf) {
    float3 color = 0.0;
    for (int i = 0; i < GetDirectionalLightCount(); i++) {
        color += _GetLighting(surface, brdf, GetDirectionalLight(i,surface));
    }
    return color;
}

#endif