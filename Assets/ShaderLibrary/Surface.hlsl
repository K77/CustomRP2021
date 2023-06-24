#ifndef CUSTOM_SURFACE_INCLUDED
#define CUSTOM_SURFACE_INCLUDED

struct Surface {
    float3 position;
    float3 normal;
    float3 viewDirection;
    float depth;
    float3 color;
    float alpha;
    //金属度，等同于Specular反射率，表示高光和漫反射的比率
    //等于1的时候可以认为几乎都是高光，没有漫反射
    float metallic;         
    float smoothness;
};

#endif