using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Lighting {

    static int
        dirLightColorId = Shader.PropertyToID("_DirectionalLightColor"),
        dirLightDirectionId = Shader.PropertyToID("_DirectionalLightDirection");
    const string bufferName = "Lighting";

    CommandBuffer buffer = new CommandBuffer {
        name = bufferName
    };
    CullingResults cullingResults;
    Shadows shadows = new Shadows();
    public void Setup (ScriptableRenderContext context,CullingResults cullingResults,ShadowSettings shadowSettings)
    {
        this.cullingResults = cullingResults;
        buffer.BeginSample(bufferName);
        shadows.Setup(context, cullingResults, shadowSettings);
        SetupLights();
        // SetupDirectionalLight();
        buffer.EndSample(bufferName);
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    void SetupDirectionalLight (int index, VisibleLight visibleLight) {
        dirLightColors[index] = visibleLight.finalColor;
        dirLightDirections[index] = -visibleLight.localToWorldMatrix.GetColumn(2);
    }
    
    const int maxDirLightCount = 4;

    static int
        //dirLightColorId = Shader.PropertyToID("_DirectionalLightColor"),
        //dirLightDirectionId = Shader.PropertyToID("_DirectionalLightDirection");
        dirLightCountId = Shader.PropertyToID("_DirectionalLightCount"),
        dirLightColorsId = Shader.PropertyToID("_DirectionalLightColors"),
        dirLightDirectionsId = Shader.PropertyToID("_DirectionalLightDirections");

    static Vector4[]
        dirLightColors = new Vector4[maxDirLightCount],
        dirLightDirections = new Vector4[maxDirLightCount];
    void SetupLights () {
        NativeArray<VisibleLight> visibleLights = cullingResults.visibleLights;
    }

}