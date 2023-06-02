#ifndef CUSTOM_COMMON_INCLUDED
#define CUSTOM_COMMON_INCLUDED

#include "UnityInput.hlsl"

#define UNITY_MATRIX_M unity_ObjectToWorld
#define UNITY_PREV_MATRIX_M unity_ObjectToWorld
#define UNITY_PREV_MATRIX_I_M unity_ObjectToWorld

#define UNITY_MATRIX_I_V unity_WorldToObject
#define UNITY_MATRIX_I_M unity_WorldToObject
#define UNITY_MATRIX_V unity_MatrixV
#define UNITY_MATRIX_VP unity_MatrixVP
#define UNITY_MATRIX_P glstate_matrix_projection

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

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"



// float3 TransformObjectToWorld (float3 positionOS) {
//     return mul(unity_ObjectToWorld, float4(positionOS, 1.0)).xyz;
// }
//
// float4 TransformWorldToHClip (float3 positionWS) {
//     return mul(unity_MatrixVP, float4(positionWS, 1.0));
// }
	
#endif