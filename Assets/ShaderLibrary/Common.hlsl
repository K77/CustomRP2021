#ifndef CUSTOM_COMMON_INCLUDED
#define CUSTOM_COMMON_INCLUDED

#undef SHADER_API_MOBILE //to resolve cannot convert output parameter from 'min16float[4]' to 'float[4]

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "UnityInput.hlsl"

// float4x4 unity_MatrixPreviousM;


#define UNITY_MATRIX_M unity_ObjectToWorld //
#define UNITY_MATRIX_I_M unity_WorldToObject

#define UNITY_MATRIX_V unity_MatrixV
// #define UNITY_MATRIX_I_V unity_WorldToObject

#define UNITY_MATRIX_VP unity_MatrixVP

#define UNITY_MATRIX_P glstate_matrix_projection


//使用2021版本的坑，我们还需要定义两个PREV标识符，才不会报错，但这两个变量具体代表什么未知
//https://forum.unity.com/threads/marco-redefinition-unityinstancing-hlsl-unity-matrix.1208449/
// #define UNITY_PREV_MATRIX_M    UNITY_ACCESS_INSTANCED_PROP(unity_Builtins3, unity_PrevObjectToWorldArray)
// #define UNITY_PREV_MATRIX_M unity_PrevObjectToWorldArray
// #define UNITY_PREV_MATRIX_I_M unity_PrevWorldToObjectArray
// #define UNITY_PREV_MATRIX_M unity_ObjectToWorld
// #define UNITY_PREV_MATRIX_I_M unity_WorldToObject

#define UNITY_PREV_MATRIX_M   unity_MatrixPreviousM
#define UNITY_PREV_MATRIX_I_M unity_MatrixPreviousMI

// float4x4 GetObjectToWorldMatrix()
// {
// 	return UNITY_MATRIX_M;
// }
//
// float4x4 GetWorldToObjectMatrix()
// {
// 	return UNITY_MATRIX_I_M;
// }

// float4x4 GetPrevObjectToWorldMatrix()
// {
// 	return UNITY_PREV_MATRIX_M;
// }
//
// float4x4 GetPrevWorldToObjectMatrix()
// {
// 	return UNITY_PREV_MATRIX_I_M;
// }

// float4x4 GetWorldToViewMatrix()
// {
// 	return UNITY_MATRIX_V;
// }

// float4x4 GetViewToWorldMatrix()
// {
// 	return UNITY_MATRIX_I_V;
// }

// Transform to homogenous clip space
// float4x4 GetWorldToHClipMatrix()
// {
// 	return UNITY_MATRIX_VP;
// }
//
// // Transform to homogenous clip space
// float4x4 GetViewToHClipMatrix()
// {
// 	return UNITY_MATRIX_P;
// }

// #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

float Square (float x) {
	return x * x;
}
float DistanceSquared(float3 pA, float3 pB) {
	return dot(pA - pB, pA - pB);
}
// float3 TransformObjectToWorld (float3 positionOS) {
//     return mul(unity_ObjectToWorld, float4(positionOS, 1.0)).xyz;
// }
//
// float4 TransformWorldToHClip (float3 positionWS) {
//     return mul(unity_MatrixVP, float4(positionWS, 1.0));
// }
	
#endif