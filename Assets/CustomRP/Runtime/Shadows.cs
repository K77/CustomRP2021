using UnityEngine;
using UnityEngine.Rendering;

public class Shadows {

    const string bufferName = "Shadows";
    const int maxShadowedDirectionalLightCount = 1;
    CommandBuffer buffer = new CommandBuffer {
        name = bufferName
    };
    
    struct ShadowedDirectionalLight {
        public int visibleLightIndex;
    }

    ShadowedDirectionalLight[] ShadowedDirectionalLights =
        new ShadowedDirectionalLight[maxShadowedDirectionalLightCount];

    ScriptableRenderContext context;

    CullingResults cullingResults;

    ShadowSettings settings;

    public void Setup (
        ScriptableRenderContext context, CullingResults cullingResults,
        ShadowSettings settings
    ) {
        this.context = context;
        this.cullingResults = cullingResults;
        this.settings = settings;
    }

    void ExecuteBuffer () {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
}