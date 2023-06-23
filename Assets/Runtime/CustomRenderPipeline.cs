using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomRenderPipeline : RenderPipeline
{
    private CameraRenderer _renderer = new CameraRenderer();
    private bool useDynamicBatching;
    private bool useGPUInstancing;
    private bool useSRPBatcher;

    private ShadowSettings shadowSettings;
    public CustomRenderPipeline(
        bool useDynamicBatching, bool useGPUInstancing, bool useSRPBatcher,
        ShadowSettings shadowSettings)
    {
        this.useDynamicBatching = useDynamicBatching;
        this.useGPUInstancing = useGPUInstancing;
        
        GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;
        
        this.shadowSettings = shadowSettings;
        // GraphicsSettings.useScriptableRenderPipelineBatching = true;
    }

    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        foreach (var camera in cameras)
        {
            _renderer.Render(context,camera,useDynamicBatching,useGPUInstancing,shadowSettings);
        }
    }
}
