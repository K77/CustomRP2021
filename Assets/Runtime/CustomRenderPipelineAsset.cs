using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/CreateCustomRenderPipeline")]
public class CustomRenderPipelineAsset : RenderPipelineAsset
{
    [SerializeField]
    bool useDynamicBatching = false, useGPUInstancing = false, useSRPBatcher = true;
    
    [SerializeField]
    ShadowSettings shadows = default;
    
    protected override RenderPipeline CreatePipeline()
    {
        return new CustomRenderPipeline(useDynamicBatching,useGPUInstancing,useSRPBatcher,shadows);
    }
}
