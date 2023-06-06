using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/CreateCustomRenderPipeline")]
public class CustomRenderPipelineAsset : RenderPipelineAsset
{

    [SerializeField]
    ShadowSettings shadows = default;
    
    protected override RenderPipeline CreatePipeline()
    {
        return new CustomRenderPipeline(true,false,true,shadows);
    }
}
