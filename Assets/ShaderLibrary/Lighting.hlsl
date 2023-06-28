#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

float3 _IncomingLight (Surface surface, Light light) {
    //saturate(v): 将v夹取到 [0,1]区间.
    return saturate(dot(surface.normal, light.direction) * light.attenuation) * light.color;
}

float3 _GetLighting (Surface surface, BRDF brdf, Light light) {
    return _IncomingLight(surface, light) * DirectBRDF(surface, brdf, light);
}

float3 GetLighting (Surface surfaceWS, BRDF brdf, GI gi) {
    ShadowData shadowData = GetShadowData(surfaceWS);
    float3 color = gi.diffuse * brdf.diffuse;//这个就直接没看懂， brdf到底是什么？？？
    for (int i = 0; i < GetDirectionalLightCount(); i++) {
        Light light = GetDirectionalLight(i, surfaceWS, shadowData);
        color += _GetLighting(surfaceWS, brdf, light);
    }
    return color;
}

#endif